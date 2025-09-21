# Script para configurar el almacenamiento del estado de Terraform en Azure Storage

# Variables
$RESOURCE_GROUP_NAME = "terraform-state-rg"
$LOCATION = "westus"
$RANDOM_SUFFIX = Get-Random -Minimum 10000 -Maximum 99999
$STORAGE_ACCOUNT_NAME = "tfstate$RANDOM_SUFFIX"
$CONTAINER_NAME = "tfstate"

# Verificar si el usuario está conectado a Azure
$context = Get-AzContext
if (!$context) {
    Write-Host "No has iniciado sesión en Azure. Ejecutando Connect-AzAccount..."
    Connect-AzAccount
}

# Crear grupo de recursos
Write-Host "Creando grupo de recursos $RESOURCE_GROUP_NAME..."
New-AzResourceGroup -Name $RESOURCE_GROUP_NAME -Location $LOCATION -Force

# Crear cuenta de almacenamiento
Write-Host "Creando cuenta de almacenamiento $STORAGE_ACCOUNT_NAME..."
$storageAccount = New-AzStorageAccount `
    -ResourceGroupName $RESOURCE_GROUP_NAME `
    -Name $STORAGE_ACCOUNT_NAME `
    -Location $LOCATION `
    -SkuName Standard_LRS `
    -Kind StorageV2

# Obtener clave de la cuenta de almacenamiento
$ACCOUNT_KEY = (Get-AzStorageAccountKey -ResourceGroupName $RESOURCE_GROUP_NAME -Name $STORAGE_ACCOUNT_NAME)[0].Value

# Crear contexto de almacenamiento
$ctx = New-AzStorageContext -StorageAccountName $STORAGE_ACCOUNT_NAME -StorageAccountKey $ACCOUNT_KEY

# Crear contenedor de blob
Write-Host "Creando contenedor de blob $CONTAINER_NAME..."
New-AzStorageContainer -Name $CONTAINER_NAME -Context $ctx -Permission Off

# Mostrar configuración para backend.tf
Write-Host ""
Write-Host "Configuración para backend.tf:"
Write-Host "terraform {" -ForegroundColor Green
Write-Host "  backend \"azurerm\" {" -ForegroundColor Green
Write-Host "    resource_group_name  = \"$RESOURCE_GROUP_NAME\"" -ForegroundColor Green
Write-Host "    storage_account_name = \"$STORAGE_ACCOUNT_NAME\"" -ForegroundColor Green
Write-Host "    container_name       = \"$CONTAINER_NAME\"" -ForegroundColor Green
Write-Host "    key                  = \"event-ticketing.terraform.tfstate\"" -ForegroundColor Green
Write-Host "  }" -ForegroundColor Green
Write-Host "}" -ForegroundColor Green

Write-Host ""
Write-Host "Guarda esta configuración y actualiza el archivo backend.tf" -ForegroundColor Yellow
Write-Host "Luego ejecuta 'terraform init' para inicializar el backend remoto" -ForegroundColor Yellow