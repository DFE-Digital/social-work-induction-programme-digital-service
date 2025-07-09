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
  type        = string
  description = "The Moodle app service plan SKU"
  default     = "S3"
}

variable "asp_sku_services" {
  type        = string
  description = "The services app service plan SKU"
  default     = "S3"
}

variable "asp_sku_maintenance" {
  type        = string
  description = "The maintenance app service plan SKU"
  default     = "S3"
}

variable "moodle_default_instances" {
  type        = number
  description = "The default number of instances for the Moodle app service"
  default     = 1
}

variable "moodle_minimum_instances" {
  type        = number
  description = "The minimum number of instances for the Moodle app service"
  default     = 1
}

variable "moodle_maximum_instances" {
  type        = number
  description = "The maximum number of instances for the Moodle app service"
  default     = 1
}

variable "service_apps_default_instances" {
  type        = number
  description = "The default number of instances for the service apps app service"
  default     = 1
}

variable "service_apps_minimum_instances" {
  type        = number
  description = "The minimum number of instances for the service apps app service"
  default     = 1
}

variable "service_apps_maximum_instances" {
  type        = number
  description = "The maximum number of instances for the service apps app service"
  default     = 1
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

variable "basic_auth_password" {
  description = "Password for basic auth protected sites"
  type        = string
  sensitive   = true
}

variable "email_support_address" {
  description = "Email address for support / alerting"
  type        = string
  sensitive   = true
}

variable "asp_sku_notification" {
  description = "The app service plan SKU"
  type        = string
}

variable "moodle_max_data_storage_size_in_gb" {
  type        = number
  description = "The provisioned size (quota) in GiB for the Moodle data file share. This directly impacts performance and cost for Premium shares."
  default     = 5

  validation {
    condition     = var.moodle_max_data_storage_size_in_gb > 0
    error_message = "The storage size must be a positive number greater than 0."
  }
}

variable "storage_redundancy" {
  type        = string
  description = "The redundancy type for the storage accounts. Allowed values are LRS, ZRS, GRS, GZRS, RA-GRS, RA-GZRS."
  default     = "LRS"

  validation {
    condition = contains([
      "LRS",
      "ZRS",
      "GRS",
      "GZRS",
      "RA-GRS",
      "RA-GZRS"
    ], var.storage_redundancy)
    error_message = "Allowed values for storage_redundancy are LRS, ZRS, GRS, GZRS, RA-GRS, or RA-GZRS."
  }
}

variable "blob_storage_account_tier" {
  type        = string
  description = "The account tier for the general-purpose blob storage account. Allowed values are Standard or Premium."
  default     = "Standard"

  validation {
    condition     = contains(["Standard", "Premium"], var.blob_storage_account_tier)
    error_message = "Allowed values for blob_storage_account_tier are Standard or Premium."
  }
}

variable "file_storage_account_tier" {
  type        = string
  description = "The account tier for the dedicated Moodle file storage account. Allowed values are Standard or Premium."
  default     = "Standard"

  validation {
    condition     = contains(["Standard", "Premium"], var.file_storage_account_tier)
    error_message = "Allowed values for file_storage_account_tier are Standard or Premium."
  }
}
