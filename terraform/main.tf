terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 4.0.0"
    }
    random = {
      source  = "hashicorp/random"
      version = "~> 3.1.0"
    }
  }
  required_version = ">= 0.14.9"
}

provider "azurerm" {
  features {}
  subscription_id = var.subscription_id
}

# Generate a random integer to create a globally unique name
resource "random_integer" "ri" {
  min = 100
  max = 999
}

# Create the resource group
resource "azurerm_resource_group" "rg" {
  name     = "event-ticketing-rg-${random_integer.ri.result}"
  location = "westus"
  tags = {
    environment = "production"
    project     = "event-ticketing"
  }
}

# Create PostgreSQL Flexible Server for Supabase
resource "azurerm_postgresql_flexible_server" "postgres" {
  name                   = "supabase-pg-${random_integer.ri.result}"
  resource_group_name    = azurerm_resource_group.rg.name
  location               = azurerm_resource_group.rg.location
  version                = "14"
  administrator_login    = var.postgres_admin_username
  administrator_password = var.postgres_admin_password
  storage_mb             = 32768
  sku_name               = "B_Standard_B1ms"
  zone                   = "1"

  tags = {
    environment = "production"
    project     = "event-ticketing"
  }
}

# Create PostgreSQL Database
resource "azurerm_postgresql_flexible_server_database" "database" {
  name      = "event_ticketing"
  server_id = azurerm_postgresql_flexible_server.postgres.id
  charset   = "UTF8"
  collation = "en_US.utf8"
}

# Allow all Azure services to access the PostgreSQL server
resource "azurerm_postgresql_flexible_server_firewall_rule" "allow_azure_services" {
  name             = "AllowAllAzureServices"
  server_id        = azurerm_postgresql_flexible_server.postgres.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "0.0.0.0"
}

# Create App Service Plan
resource "azurerm_service_plan" "app_plan" {
  name                = "event-ticketing-plan-${random_integer.ri.result}"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  os_type             = "Linux"
  sku_name            = "B1"

  tags = {
    environment = "production"
    project     = "event-ticketing"
  }
}

# Create Web App for API
resource "azurerm_linux_web_app" "api" {
  name                = "event-ticketing-api-${random_integer.ri.result}"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  service_plan_id     = azurerm_service_plan.app_plan.id

  site_config {
    application_stack {
      dotnet_version = "7.0"
    }
    always_on = true
  }

  app_settings = {
    "ASPNETCORE_ENVIRONMENT" = "Production"
    "SUPABASE_CONNECTION_STRING" = join("", [
      "Host=${azurerm_postgresql_flexible_server.postgres.fqdn};",
      "Database=${azurerm_postgresql_flexible_server_database.database.name};",
      "Username=${var.postgres_admin_username};",
      "Password=${var.postgres_admin_password}"
    ])
    "CORS_ORIGINS" = "https://${azurerm_storage_account.frontend.primary_web_host}"
  }

  tags = {
    environment = "production"
    project     = "event-ticketing"
  }
}

# Create Storage Account for Frontend
resource "azurerm_storage_account" "frontend" {
  name                     = "eventticketing${random_integer.ri.result}"
  resource_group_name      = azurerm_resource_group.rg.name
  location                 = azurerm_resource_group.rg.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
  account_kind             = "StorageV2"

  static_website {
    index_document     = "index.html"
    error_404_document = "index.html"
  }

  tags = {
    environment = "production"
    project     = "event-ticketing"
  }
}