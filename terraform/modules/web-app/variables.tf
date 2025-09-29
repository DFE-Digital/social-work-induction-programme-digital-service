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

variable "magic_links_enabled" {
  description = "Whether Magic Links authentication is enabled for this environment"
  type        = bool
  default     = false
}

variable "magic_link_token_value" {
  description = "Shared magic link token value"
  type        = string
  sensitive   = true
  default     = null
}



variable "magic_link_rule_set_id" {
  description = "ID of shared magic link rule set"
  type        = string
  default     = null
}

variable "subnet_webapps_id" {
  description = "The ID of the web apps subnet"
  type        = string
}

variable "docker_image_name" {
  description = "The Docker image name to use for the web app"
  type        = string
}

variable "health_check_path" {
  description = "The path to use for the health check endpoint. Leave unset to disable"
  type        = string
  default     = ""
}

variable "health_check_eviction_time_in_min" {
  description = "Time in minutes to wait before evicting a failed instance app instance"
  type        = number
  default     = 2
}

variable "support_action_group_id" {
  description = "ID of the support action group"
  type        = string
}

variable "storage_mounts" {
  type = map(object({
    type         = string
    mount_path   = string
    account_name = string
    share_name   = string
  }))
  description = "A map of storage mounts to be configured for the App Service. The key of the map is used as the mount's configuration name. An empty map creates no mounts."
  default     = {} # Default to an empty map, making it optional.
}

# Separate the access key from the above storage_mounts variable - you can't declare a for each
# variable with a sensitive value
variable "storage_access_key" {
  description = "Storage access key for the storage mount"
  default     = ""
  type        = string
  sensitive   = true
}
