# Outputs from Terraform deployment

output "resource_group_name" {
  value       = azurerm_resource_group.rg.name
  description = "The name of the resource group"
}

output "api_url" {
  value       = "https://${azurerm_linux_web_app.api.default_hostname}"
  description = "The URL of the deployed API"
}

output "frontend_url" {
  value       = "https://${azurerm_storage_account.frontend.primary_web_host}"
  description = "The URL of the static website hosting the frontend"
}



output "postgres_server_name" {
  value       = azurerm_postgresql_flexible_server.postgres.name
  description = "The name of the PostgreSQL server"
}

output "postgres_connection_string" {
  value       = "Host=${azurerm_postgresql_flexible_server.postgres.fqdn};Database=${azurerm_postgresql_flexible_server_database.database.name};Username=${var.postgres_admin_username};Password=${var.postgres_admin_password}"
  description = "The connection string for the PostgreSQL database"
  sensitive   = true
}