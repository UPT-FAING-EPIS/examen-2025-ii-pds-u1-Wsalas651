# Script para configurar recursos en Azure para el despliegue de la aplicación de venta de entradas

# Parámetros configurables
param (
    [string]$resourceGroupName = "EventTicketingRG",
    [string]$location = "westus",
    [string]$appServicePlanName = "EventTicketingPlan",
    [string]$webAppName = "eticketing-app", # Modificado para evitar conflicto de nombres
    [string]$storageAccountName = "eventticketingstore",
    [string]$servicePrincipalName = "EventTicketingGitHubActions",
    [string]$postgresServerName = "eticketing-postgres", # Nuevo parámetro para servidor PostgreSQL
    [string]$postgresAdminUsername = "postgres", # Usuario administrador de PostgreSQL
    [string]$databaseName = "event_ticketing" # Nombre de la base de datos
)

# Función para verificar si el usuario está autenticado en Azure
function Test-AzureLogin {
    try {
        $context = Get-AzContext
        if (-not $context) {
            Write-Host "No estás autenticado en Azure. Iniciando proceso de autenticación..." -ForegroundColor Yellow
            Connect-AzAccount
        } else {
            Write-Host "Ya estás autenticado en Azure como: $($context.Account)" -ForegroundColor Green
        }
    } catch {
        Write-Host "Error al verificar la autenticación de Azure. Asegúrate de tener instalado el módulo Az." -ForegroundColor Red
        exit
    }
}

# Función para crear un grupo de recursos
function New-ResourceGroupIfNotExists {
    param (
        [string]$resourceGroupName,
        [string]$location
    )
    
    $resourceGroup = Get-AzResourceGroup -Name $resourceGroupName -ErrorAction SilentlyContinue
    
    if (-not $resourceGroup) {
        Write-Host "Creando grupo de recursos '$resourceGroupName' en la ubicación '$location'..." -ForegroundColor Yellow
        New-AzResourceGroup -Name $resourceGroupName -Location $location
        Write-Host "Grupo de recursos creado exitosamente." -ForegroundColor Green
    } else {
        Write-Host "El grupo de recursos '$resourceGroupName' ya existe." -ForegroundColor Green
    }
}

# Función para crear un plan de App Service
function New-AppServicePlanIfNotExists {
    param (
        [string]$appServicePlanName,
        [string]$resourceGroupName,
        [string]$location
    )
    
    $appServicePlan = Get-AzAppServicePlan -Name $appServicePlanName -ResourceGroupName $resourceGroupName -ErrorAction SilentlyContinue
    
    if (-not $appServicePlan) {
        Write-Host "Creando plan de App Service '$appServicePlanName'..." -ForegroundColor Yellow
        New-AzAppServicePlan -Name $appServicePlanName -ResourceGroupName $resourceGroupName -Location $location -Tier "B1"
        Write-Host "Plan de App Service creado exitosamente." -ForegroundColor Green
    } else {
        Write-Host "El plan de App Service '$appServicePlanName' ya existe." -ForegroundColor Green
    }
}

# Función para crear una Web App
function New-WebAppIfNotExists {
    param (
        [string]$webAppName,
        [string]$resourceGroupName,
        [string]$appServicePlanName
    )
    
    $webApp = Get-AzWebApp -Name $webAppName -ResourceGroupName $resourceGroupName -ErrorAction SilentlyContinue
    
    if (-not $webApp) {
        Write-Host "Creando Web App '$webAppName'..." -ForegroundColor Yellow
        try {
            New-AzWebApp -Name $webAppName -ResourceGroupName $resourceGroupName -AppServicePlan $appServicePlanName
            Write-Host "Web App creada exitosamente." -ForegroundColor Green
        } catch {
            Write-Host "Error al crear la Web App: $_" -ForegroundColor Red
            Write-Host "Intenta con un nombre diferente modificando el parámetro webAppName." -ForegroundColor Yellow
        }
    } else {
        Write-Host "La Web App '$webAppName' ya existe." -ForegroundColor Green
    }
}

# Función para crear una cuenta de almacenamiento
function New-StorageAccountIfNotExists {
    param (
        [string]$storageAccountName,
        [string]$resourceGroupName,
        [string]$location
    )
    
    $storageAccount = Get-AzStorageAccount -Name $storageAccountName -ResourceGroupName $resourceGroupName -ErrorAction SilentlyContinue
    
    if (-not $storageAccount) {
        Write-Host "Creando cuenta de almacenamiento '$storageAccountName'..." -ForegroundColor Yellow
        $storageAccount = New-AzStorageAccount -Name $storageAccountName -ResourceGroupName $resourceGroupName -Location $location -SkuName "Standard_LRS" -Kind "StorageV2"
        Write-Host "Cuenta de almacenamiento creada exitosamente." -ForegroundColor Green
        
        # Habilitar sitio web estático
        Write-Host "Habilitando sitio web estático..." -ForegroundColor Yellow
        Enable-AzStorageStaticWebsite -Context $storageAccount.Context -IndexDocument "index.html" -ErrorDocument "index.html"
        Write-Host "Sitio web estático habilitado exitosamente." -ForegroundColor Green
        
        # Obtener la URL del sitio web estático
        $staticWebsiteUrl = $storageAccount.PrimaryEndpoints.Web
        Write-Host "URL del sitio web estático: $staticWebsiteUrl" -ForegroundColor Cyan
    } else {
        Write-Host "La cuenta de almacenamiento '$storageAccountName' ya existe." -ForegroundColor Green
        
        # Asegurarse de que el sitio web estático esté habilitado
        Enable-AzStorageStaticWebsite -Context $storageAccount.Context -IndexDocument "index.html" -ErrorDocument "index.html" -ErrorAction SilentlyContinue
        
        # Obtener la URL del sitio web estático
        $staticWebsiteUrl = $storageAccount.PrimaryEndpoints.Web
        Write-Host "URL del sitio web estático: $staticWebsiteUrl" -ForegroundColor Cyan
    }
    
    return $storageAccount
}



# Función para crear un servidor PostgreSQL flexible
function New-PostgreSQLFlexibleServerIfNotExists {
    param (
        [string]$serverName,
        [string]$resourceGroupName,
        [string]$location,
        [string]$adminUsername,
        [string]$databaseName
    )
    
    # Verificar si el servidor ya existe
    $server = Get-AzPostgreSqlFlexibleServer -ResourceGroupName $resourceGroupName -ServerName $serverName -ErrorAction SilentlyContinue
    
    if (-not $server) {
        Write-Host "Creando servidor PostgreSQL flexible '$serverName'..." -ForegroundColor Yellow
        
        # Generar una contraseña segura
        $adminPassword = [System.Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes((New-Guid).Guid)) + "!1aA"
        
        try {
            # Crear el servidor PostgreSQL
            $server = New-AzPostgreSqlFlexibleServer `
                -ResourceGroupName $resourceGroupName `
                -Name $serverName `
                -Location $location `
                -AdministratorUsername $adminUsername `
                -AdministratorLoginPassword (ConvertTo-SecureString $adminPassword -AsPlainText -Force) `
                -Sku Standard_B1ms `
                -Version 13
            
            Write-Host "Servidor PostgreSQL flexible creado exitosamente." -ForegroundColor Green
            
            # Crear la base de datos
            New-AzPostgreSqlFlexibleServerDatabase -ResourceGroupName $resourceGroupName -ServerName $serverName -Name $databaseName
            Write-Host "Base de datos '$databaseName' creada exitosamente." -ForegroundColor Green
            
            # Permitir acceso desde servicios de Azure
            Update-AzPostgreSqlFlexibleServerFirewallRule -ResourceGroupName $resourceGroupName -ServerName $serverName -Name "AllowAllAzureIPs" -StartIPAddress "0.0.0.0" -EndIPAddress "0.0.0.0"
            Write-Host "Regla de firewall para permitir acceso desde servicios de Azure configurada." -ForegroundColor Green
            
            # Mostrar la cadena de conexión
            $connectionString = "Host=$serverName.postgres.database.azure.com;Database=$databaseName;Username=$adminUsername;Password=$adminPassword"
            Write-Host "Cadena de conexión a PostgreSQL: $connectionString" -ForegroundColor Cyan
            
            # Guardar la cadena de conexión para referencia
            $connectionString | Out-File -FilePath "./postgres_connection.txt"
            Write-Host "La cadena de conexión se ha guardado en el archivo 'postgres_connection.txt'." -ForegroundColor Yellow
            Write-Host "IMPORTANTE: Este archivo contiene información sensible. No lo compartas ni lo subas a un repositorio." -ForegroundColor Red
            
            return @{
                "ConnectionString" = $connectionString
                "AdminUsername" = $adminUsername
                "AdminPassword" = $adminPassword
            }
        } catch {
            Write-Host "Error al crear el servidor PostgreSQL: $_" -ForegroundColor Red
        }
    } else {
        Write-Host "El servidor PostgreSQL '$serverName' ya existe." -ForegroundColor Green
        return $null
    }
}

# Función para crear un Service Principal para GitHub Actions
function New-ServicePrincipalForGitHubActions {
    param (
        [string]$servicePrincipalName,
        [string]$resourceGroupName
    )
    
    Write-Host "Creando Service Principal para GitHub Actions..." -ForegroundColor Yellow
    
    $subscriptionId = (Get-AzContext).Subscription.Id
    $scope = "/subscriptions/$subscriptionId/resourceGroups/$resourceGroupName"
    
    # Crear el Service Principal con el rol de Contributor en el grupo de recursos
    try {
        $sp = New-AzADServicePrincipal -DisplayName $servicePrincipalName -Role "Contributor" -Scope $scope
        
        # Crear credenciales para el Service Principal
        $credentials = New-Object -TypeName PSObject -Property @{
            clientId = $sp.AppId
            clientSecret = [System.Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes((New-Guid).Guid))
            subscriptionId = $subscriptionId
            tenantId = (Get-AzContext).Tenant.Id
        }
        
        # Convertir a JSON
        $credentialsJson = $credentials | ConvertTo-Json
        
        Write-Host "Service Principal creado exitosamente." -ForegroundColor Green
        Write-Host "`nCopia el siguiente JSON y guárdalo como secreto 'AZURE_CREDENTIALS' en GitHub:`n" -ForegroundColor Yellow
        Write-Host $credentialsJson -ForegroundColor Cyan
        
        # Guardar en un archivo para referencia
        $credentialsJson | Out-File -FilePath "./azure_credentials.json"
        Write-Host "`nLas credenciales también se han guardado en el archivo 'azure_credentials.json'." -ForegroundColor Yellow
        Write-Host "IMPORTANTE: Este archivo contiene información sensible. No lo compartas ni lo subas a un repositorio." -ForegroundColor Red
    } catch {
        Write-Host "Error al crear el Service Principal: $_" -ForegroundColor Red
    }
}

# Función principal
function Main {
    # Verificar si el módulo Az está instalado
    if (-not (Get-Module -ListAvailable -Name Az)) {
        Write-Host "El módulo Az no está instalado. Instalando..." -ForegroundColor Yellow
        Install-Module -Name Az -Scope CurrentUser -Repository PSGallery -Force
    }
    
    # Verificar si el módulo Az.PostgreSql está instalado
    if (-not (Get-Module -ListAvailable -Name Az.PostgreSql)) {
        Write-Host "El módulo Az.PostgreSql no está instalado. Instalando..." -ForegroundColor Yellow
        Install-Module -Name Az.PostgreSql -Scope CurrentUser -Repository PSGallery -Force
    }
    
    # Importar los módulos necesarios
    Import-Module Az
    Import-Module Az.PostgreSql
    
    # Verificar autenticación en Azure
    Test-AzureLogin
    
    # Crear recursos
    New-ResourceGroupIfNotExists -resourceGroupName $resourceGroupName -location $location
    New-AppServicePlanIfNotExists -appServicePlanName $appServicePlanName -resourceGroupName $resourceGroupName -location $location
    New-WebAppIfNotExists -webAppName $webAppName -resourceGroupName $resourceGroupName -appServicePlanName $appServicePlanName
    $storageAccount = New-StorageAccountIfNotExists -storageAccountName $storageAccountName -resourceGroupName $resourceGroupName -location $location
    $postgresInfo = New-PostgreSQLFlexibleServerIfNotExists -serverName $postgresServerName -resourceGroupName $resourceGroupName -location $location -adminUsername $postgresAdminUsername -databaseName $databaseName
    
    # Configurar la cadena de conexión en la Web App si se creó el servidor PostgreSQL
    if ($postgresInfo) {
        Write-Host "Configurando la cadena de conexión en la Web App..." -ForegroundColor Yellow
        $connectionString = $postgresInfo.ConnectionString
        Set-AzWebApp -ResourceGroupName $resourceGroupName -Name $webAppName -AppSettings @{"SUPABASE_CONNECTION_STRING" = $connectionString}
        Write-Host "Cadena de conexión configurada exitosamente en la Web App." -ForegroundColor Green
    }
    
    # Crear Service Principal para GitHub Actions
    $createServicePrincipal = Read-Host "¿Deseas crear un Service Principal para GitHub Actions? (S/N)"
    if ($createServicePrincipal -eq "S" -or $createServicePrincipal -eq "s") {
        New-ServicePrincipalForGitHubActions -servicePrincipalName $servicePrincipalName -resourceGroupName $resourceGroupName
    }
    
    # Mostrar resumen
    Write-Host "`n===== RESUMEN DE RECURSOS CREADOS =====" -ForegroundColor Green
    Write-Host "Grupo de recursos: $resourceGroupName" -ForegroundColor White
    Write-Host "Plan de App Service: $appServicePlanName" -ForegroundColor White
    Write-Host "Web App (API): $webAppName" -ForegroundColor White
    Write-Host "URL de la API: https://$webAppName.azurewebsites.net" -ForegroundColor White
    Write-Host "Cuenta de almacenamiento (Frontend): $storageAccountName" -ForegroundColor White
    Write-Host "URL del sitio web estático: $($storageAccount.PrimaryEndpoints.Web)" -ForegroundColor White
    if ($postgresInfo) {
        Write-Host "Servidor PostgreSQL: $postgresServerName" -ForegroundColor White
        Write-Host "Base de datos: $databaseName" -ForegroundColor White
    }
    
    Write-Host "`n===== PRÓXIMOS PASOS =====" -ForegroundColor Yellow
    Write-Host "1. Configura los siguientes secretos en GitHub:" -ForegroundColor White
    Write-Host "   - AZURE_CREDENTIALS: El JSON del Service Principal" -ForegroundColor White
    Write-Host "   - AZURE_WEBAPP_NAME: $webAppName" -ForegroundColor White
    Write-Host "   - AZURE_STORAGE_ACCOUNT: $storageAccountName" -ForegroundColor White
    Write-Host "   - AZURE_STORAGE_KEY: (Obtén la clave desde el Portal de Azure)" -ForegroundColor White
    Write-Host "   - RESOURCE_GROUP_NAME: $resourceGroupName" -ForegroundColor White
    if ($postgresInfo) {
        Write-Host "   - SUPABASE_CONNECTION_STRING: (Usa el valor guardado en postgres_connection.txt)" -ForegroundColor White
    }
    Write-Host "2. Configura los secretos de la aplicación (JWT_SECRET_KEY, etc.)" -ForegroundColor White
    Write-Host "3. Ejecuta el flujo de trabajo de GitHub Actions para desplegar la aplicación" -ForegroundColor White
    Write-Host "4. Ejecuta el script SQL de configuración de Supabase en la base de datos PostgreSQL" -ForegroundColor White
}

# Ejecutar la función principal
Main
