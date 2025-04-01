variable "location" {
  description = "Name of the Azure region to deploy resources"
  type        = string
}

variable "resource_group" {
  description = "Name of the Azure Resource Group to deploy resources"
  type        = string
}

variable "webapp_storage_account_name" {
  description = "Storage Account name"
  type        = string
}

variable "kv_id" {
  description = "The ID of the Key Vault"
  type        = string
}

variable "tags" {
  description = "Resource tags"
  type        = map(string)
}
