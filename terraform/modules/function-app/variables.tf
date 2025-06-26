variable "environment" {
  description = "Environment to deploy resources"
  type        = string
}

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

variable "service_plan_id" {
  description = "ID of the app service plan"
  type        = string
}

variable "acr_name" {
  description = "Azure Container Registry name"
  type        = string
}

variable "acr_id" {
  description = "ID of the Container Registry"
  type        = string
}

variable "function_app_name" {
  description = "Name of the function app"
  type        = string
  default     = ""
}

variable "docker_image_name" {
  description = "The Docker image name to use for the web app"
  type        = string
}

variable "storage_account_name" {
  description = "The name of the storage account"
  type        = string
}

variable "storage_account_access_key" {
  description = "The primary access key for the storage account"
  type        = string
}

variable "appinsights_connection_string" {
  description = "Application Insights connection string"
  type        = string
}
