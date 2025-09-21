#!/bin/bash

# Script para configurar el almacenamiento del estado de Terraform en Azure Storage

# Variables
RESOURCE_GROUP_NAME="terraform-state-rg"
LOCATION="westus"
STORAGE_ACCOUNT_NAME="tfstate$RANDOM"
CONTAINER_NAME="tfstate"

# Crear grupo de recursos
az group create --name $RESOURCE_GROUP_NAME --location $LOCATION

# Crear cuenta de almacenamiento
az storage account create \
  --resource-group $RESOURCE_GROUP_NAME \
  --name $STORAGE_ACCOUNT_NAME \
  --sku Standard_LRS \
  --encryption-services blob

# Obtener clave de la cuenta de almacenamiento
ACCOUNT_KEY=$(az storage account keys list \
  --resource-group $RESOURCE_GROUP_NAME \
  --account-name $STORAGE_ACCOUNT_NAME \
  --query '[0].value' -o tsv)

# Crear contenedor de blob
az storage container create \
  --name $CONTAINER_NAME \
  --account-name $STORAGE_ACCOUNT_NAME \
  --account-key $ACCOUNT_KEY

# Mostrar configuración para backend.tf
echo "Configuración para backend.tf:"
echo "terraform {"
echo "  backend \"azurerm\" {"
echo "    resource_group_name  = \"$RESOURCE_GROUP_NAME\""
echo "    storage_account_name = \"$STORAGE_ACCOUNT_NAME\""
echo "    container_name       = \"$CONTAINER_NAME\""
echo "    key                  = \"event-ticketing.terraform.tfstate\""
echo "  }"
echo "}"

echo "\nGuarda esta configuración y actualiza el archivo backend.tf"