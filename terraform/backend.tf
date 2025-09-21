# Configure Terraform backend to store state in Azure Storage
# Uncomment and configure after creating the storage account and container

# terraform {
#   backend "azurerm" {
#     resource_group_name  = "terraform-state-rg"
#     storage_account_name = "terraformstate12345"
#     container_name       = "tfstate"
#     key                  = "event-ticketing.terraform.tfstate"
#   }
# }