variable "tenant_id" {
  description = "Azure tenant ID"
  type        = string
}

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

variable "acr_id" {
  description = "ID of the Container Registry"
  type        = string
}

variable "acr_name" {
  description = "Azure Container Registry name"
  type        = string
}

variable "key_vault_id" {
  description = "ID of the key vault"
  type        = string
}

variable "service_plan_id" {
  description = "ID of the app service plan"
  type        = string
}

variable "web_app_name" {
  description = "Name of the web app"
  type        = string
}

variable "web_app_short_name" {
  description = "Unique short name for the app to aid in unique naming of app related resource for multiple apps"
  type        = string  
}

variable "app_settings" {
  description = "App settings for the web app"
  type        = map(string)
}

variable "front_door_profile_web_id" {
  description = "The ID of the Front Door profile"
  type        = string
}

variable "subnet_webapps_id" {
  description = "The ID of the web apps subnet"
  type = string
}

variable "docker_image_name" {
  description = "The Docker image name to use for the web app"
  type = string
}
