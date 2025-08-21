variable "resource_group_name" {
  description = "The name of the resource group."
  type        = string
}

variable "location" {
  description = "The Azure region for the resources."
  type        = string
}

variable "container_image" {
  description = "The full name of your operations container image."
  type        = string
}


variable "vnet_subnet_id" {
  description = "The full resource ID of the subnet to integrate with."
  type        = string
}

variable "key_vault_id" {
  description = "The full resource ID of the Azure Key Vault."
  type        = string
}

variable "storage_account_id" {
  description = "The full resource ID of the Azure Storage Account."
  type        = string
}

variable "log_analytics_workspace_id" {
  description = "The ID for the Log Analytics Workspace"
  type        = string
}

variable "container_app_job_name" {
  description = "The name of the Container App Job"
  type        = string
}

variable "tags" {
  description = "Resource tags"
  type        = map(string)
}

variable "acr_id" {
  description = "The ID of the Azure Container Registry"
  type        = string
}

variable "environment_settings" {
  description = "The environment settings for the container app job"
  type        = map(string)
}

variable "container_app_env_id" {
  description = "The ID of the Container App Environment"
  type        = string
}
