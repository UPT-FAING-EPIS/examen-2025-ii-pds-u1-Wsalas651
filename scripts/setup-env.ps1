# Script para configurar variables de entorno locales de manera segura

# Función para solicitar un valor al usuario y guardarlo en el archivo de configuración
function Set-ConfigValue {
    param (
        [string]$Key,
        [string]$Prompt,
        [bool]$IsSecret = $false
    )
    
    if ($IsSecret) {
        $secureString = Read-Host -Prompt $Prompt -AsSecureString
        $bstr = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($secureString)
        $value = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($bstr)
        [System.Runtime.InteropServices.Marshal]::ZeroFreeBSTR($bstr)
    } else {
        $value = Read-Host -Prompt $Prompt
    }
    
    return $value
}

# Verificar si existe el archivo appsettings.Development.json
$appSettingsPath = "..\src\API\appsettings.Development.json"
$appSettingsExamplePath = "..\src\API\appsettings.json.example"

if (-not (Test-Path $appSettingsPath)) {
    Write-Host "Creando archivo appsettings.Development.json a partir del ejemplo..."
    Copy-Item -Path $appSettingsExamplePath -Destination $appSettingsPath
}

# Leer el contenido actual
$appSettings = Get-Content -Path $appSettingsPath -Raw | ConvertFrom-Json

# Solicitar valores al usuario
Write-Host "Configuración de variables de entorno para desarrollo local" -ForegroundColor Green
Write-Host "=================================================" -ForegroundColor Green

# Configurar cadena de conexión a Supabase
$supabaseConnection = Set-ConfigValue -Key "ConnectionStrings.DefaultConnection" -Prompt "Ingrese la cadena de conexión a Supabase" -IsSecret $true
$appSettings.ConnectionStrings.DefaultConnection = $supabaseConnection

# Configurar clave JWT
$jwtKey = Set-ConfigValue -Key "Jwt.Key" -Prompt "Ingrese la clave secreta para JWT" -IsSecret $true
$appSettings.Jwt.Key = $jwtKey

# Guardar los cambios
$appSettings | ConvertTo-Json -Depth 10 | Set-Content -Path $appSettingsPath

# Verificar si existe el archivo .env para el frontend
$envPath = "..\src\Frontend\.env"
$envExamplePath = "..\src\Frontend\.env.example"

if (-not (Test-Path $envPath)) {
    Write-Host "Creando archivo .env a partir del ejemplo..."
    Copy-Item -Path $envExamplePath -Destination $envPath
}

# Leer el contenido actual del .env
$envContent = Get-Content -Path $envPath

# Solicitar valores para el frontend
Write-Host "\nConfiguración de variables de entorno para el frontend" -ForegroundColor Green
Write-Host "=================================================" -ForegroundColor Green

$supabaseUrl = Set-ConfigValue -Key "REACT_APP_SUPABASE_URL" -Prompt "Ingrese la URL de Supabase"
$supabaseAnonKey = Set-ConfigValue -Key "REACT_APP_SUPABASE_ANON_KEY" -Prompt "Ingrese la clave anónima de Supabase" -IsSecret $true
$useSupabase = Set-ConfigValue -Key "REACT_APP_USE_SUPABASE" -Prompt "¿Usar Supabase directamente desde el frontend? (true/false)"
$apiUrl = Set-ConfigValue -Key "REACT_APP_API_URL" -Prompt "Ingrese la URL de la API backend"

# Actualizar el archivo .env
$newEnvContent = @()
foreach ($line in $envContent) {
    if ($line -match "^REACT_APP_SUPABASE_URL=") {
        $newEnvContent += "REACT_APP_SUPABASE_URL=$supabaseUrl"
    }
    elseif ($line -match "^REACT_APP_SUPABASE_ANON_KEY=") {
        $newEnvContent += "REACT_APP_SUPABASE_ANON_KEY=$supabaseAnonKey"
    }
    elseif ($line -match "^REACT_APP_USE_SUPABASE=") {
        $newEnvContent += "REACT_APP_USE_SUPABASE=$useSupabase"
    }
    elseif ($line -match "^REACT_APP_API_URL=") {
        $newEnvContent += "REACT_APP_API_URL=$apiUrl"
    }
    else {
        $newEnvContent += $line
    }
}

# Guardar los cambios en el archivo .env
$newEnvContent | Set-Content -Path $envPath

Write-Host "\nConfiguración completada exitosamente!" -ForegroundColor Green
Write-Host "Los archivos de configuración han sido actualizados con tus valores." -ForegroundColor Green
Write-Host "IMPORTANTE: Estos archivos contienen información sensible y NO deben ser compartidos ni subidos al repositorio." -ForegroundColor Yellow