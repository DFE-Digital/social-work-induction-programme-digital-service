variable "location" {
  description = "Name of the Azure region to deploy resources"
  type        = string
}

variable "resource_group" {
  description = "Name of the Azure Resource Group to deploy resources"
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

variable "acr_sku" {
  description = "Azure Container Registry SKU"
  type        = string
}

variable "public_network_access_enabled" {
  description = "Is public network access enabled"
  type        = string
}

variable "admin_enabled" {
  description = "Is ACR admin enabled?"
  type        = string
}
