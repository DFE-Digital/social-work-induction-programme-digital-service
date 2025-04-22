variable "environment" {
  description = "Environment to deploy resources"
  type        = string
}

variable "primary_resource_group" {
  description = "Name of the main resource group to deploy resources within"
  type        = string
}

variable "location" {
  description = "Name of the Azure region to deploy resources"
  type        = string
}

variable "resource_name_prefix" {
  description = "Prefix for resource names"
  type        = string
}

variable "tags" {
  description = "Resource tags"
  type        = map(string)
}

variable "asp_sku_moodle" {
  type = string
  description = "The app service plan SKU"
  default = "S3"
}

variable "moodle_default_instances" {
  type = number
  description = "The default number of instances for the Moodle app service"
  default = 1
}

variable "moodle_minimum_instances" {
  type = number
  description = "The minimum number of instances for the Moodle app service"
  default = 1
}

variable "moodle_maximum_instances" {
  type = number
  description = "The maximum number of instances for the Moodle app service"
  default = 1
}

variable "service_apps_default_instances" {
  type = number
  description = "The default number of instances for the service apps app service"
  default = 1
}

variable "service_apps_minimum_instances" {
  type = number
  description = "The minimum number of instances for the service apps app service"
  default = 1
}

variable "service_apps_maximum_instances" {
  type = number
  description = "The maximum number of instances for the service apps app service"
  default = 1
}

variable "admin_enabled" {
  description = "Is Azure Container Registry admin enabled?"
  type        = string
}

variable "webapp_storage_account_name" {
  description = "Storage Account name"
  type        = string
}

variable "kv_purge_protection_enabled" {
  description = "Whether purge protection is enabled for key vaults"
  type        = bool
}

variable "assign_delivery_team_key_vault_permissions" {
  description = "Whether to assign the delivery team key vault permissions as a convenience"
  type        = bool
}
