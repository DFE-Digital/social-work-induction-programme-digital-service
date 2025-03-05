variable "azure_region" {
  default     = "westeurope"
  description = "Name of the Azure region to deploy resources"
  type        = string
}

variable "environment" {
  default     = "development"
  description = "Environment to deploy resources"
  type        = string
}

variable "resource_name_prefix" {
  description = "Prefix for resource names"
  type        = string
}

variable "asp_sku" {
  description = "SKU name for the App Service Plan"
  type        = string
}

variable "webapp_worker_count" {
  default     = 1
  description = "Number of Workers for the App Service Plan"
  type        = string
}

variable "webapp_name" {
  description = "Name for the Web Application"
  type        = string
}

variable "webapp_storage_account_name" {
  description = "Storage Account name"
  type        = string
}

variable "days_to_expire" {
  description = "The number of days to add for password expiration"
  type        = string
}

variable "acr_sku" {
  description = "Azure Container Registry SKU"
  type        = string
}

variable "admin_enabled" {
  description = "Is ACR admin enabled?"
  type        = string
}

variable "webapp_docker_image" {
  description = "docker image name"
  type        = string
}

variable "webapp_docker_image_tag" {
  description = "docker image tag"
  type        = string
}

variable "webapp_docker_registry_url" {
  description = "Container Registry URL"
  type        = string
}
