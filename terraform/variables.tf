# Variables for Azure Terraform deployment

variable "subscription_id" {
  type        = string
  description = "Azure subscription id"
}

variable "postgres_admin_username" {
  type        = string
  description = "Administrator username for PostgreSQL server"
  default     = "postgres"
}

variable "postgres_admin_password" {
  type        = string
  description = "Administrator password for PostgreSQL server"
  sensitive   = true
}

variable "location" {
  type        = string
  description = "Azure region where resources will be created"
  default     = "westus"
}

variable "environment" {
  type        = string
  description = "Environment (dev, test, prod)"
  default     = "prod"
}

variable "project" {
  type        = string
  description = "Project name"
  default     = "event-ticketing"
}