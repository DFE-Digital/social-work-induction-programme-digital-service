variable "project_code" {
  description = "Project code"
  type        = string
}

variable "azure_region" {
  description = "Name of the Azure region to deploy resources"
  type        = string
}

variable "primary_resource_group" {
  description = "Name of the main resource group to deploy resources within"
  type        = string
}

variable "environment" {
  description = "Environment to deploy resources into"
  type        = string
}

variable "environment_tag" {
  description = "Environment tag for resources - should match the environment name dictated by central policy"
  type        = string
}

variable "parent_business_tag" {
  description = "Parent business tag for resources"
  type        = string
}

variable "product_tag" {
  description = "Product tag for resources"
  type        = string
}

variable "service_offering_tag" {
  description = "Service offering tag for resources"
  type        = string
}

variable "resource_name_prefix" {
  description = "Prefix for resource names"
  type        = string
}

variable "asp_sku_moodle" {
  description = "SKU name for the Moodle App Service Plan"
  type        = string
}

variable "asp_sku_maintenance" {
  description = "SKU name for the maintenance App Service Plan"
  type        = string
}

variable "asp_sku_services" {
  description = "SKU name for the service apps App Service Plan"
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

variable "create_and_own_container_registry" {
  description = "Whether the environment should create an own the container registry"
  type        = bool
  default     = false
}
variable "acr_name" {
  description = "Azure Container Registry name"
  type        = string
}

variable "acr_resource_group" {
  description = "Azure Container Registry resource group"
  type        = string
  default     = ""
}

variable "acr_sku" {
  description = "Azure Container Registry SKU"
  type        = string
}

variable "admin_enabled" {
  description = "Is ACR admin enabled?"
  type        = string
}

variable "moodle_db_prefix" {
  description = "The prefix for the Moodle database"
  type        = string
  default     = "mdl_"
}

variable "moodle_web_port" {
  description = "The web server port being exposed by the docker container for Moodle"
  type        = string
  default     = "8080"
}

variable "moodle_site_fullname" {
  description = "The full name of the Moodle site"
  type        = string
}

variable "moodle_site_shortname" {
  description = "Short name for the Moodle site"
  type        = string
}

variable "moodle_admin_user" {
  description = "The username for the admin account on Moodle"
  type        = string
  sensitive   = true
}

variable "moodle_admin_password" {
  description = "The password for Moodle admin user"
  type        = string
  sensitive   = true
}

variable "moodle_admin_email" {
  description = "The email address to use for the admin user on Moodle"
  type        = string
  sensitive   = true
}

variable "moodle_db_type" {
  description = "The database type for Moodle"
  type        = string
  default     = "pgsql"
}

variable "kv_purge_protection_enabled" {
  description = "Whether purge protection is enabled for key vaults"
  type        = bool
}

variable "moodle_instances" {
  description = "The names of the moodle instances to be created"
  type        = map(map(any))
}

variable "assign_delivery_team_key_vault_permissions" {
  description = "Whether to assign the delivery team key vault permissions as a convenience"
  type        = bool
}

variable "auth_service_client_id" {
  description = "Client ID for authentication into auth service"
  type        = string
  sensitive   = true
}

variable "one_login_oidc_url" {
  description = "One Login URL for OIDC integration"
  type        = string
  default     = "https://oidc.integration.account.gov.uk"
}

variable "one_login_client_id" {
  description = "The client ID for one login (non-secret)"
  type        = string
  sensitive   = true
}

variable "auth_service_app_settings" {
  description = "Environment specific auth service feature flag overrides"
  type        = map(string)
}

variable "moodle_app_settings" {
  description = "Environment specific Moodle app settings"
  type        = map(string)
  default     = {}
}

variable "user_management_app_settings" {
  description = "Environment specific user management app settings"
  type        = map(string)
  default     = {}
}

variable "moodle_web_service_name" {
  description = "Name of Moodle web service"
  type        = string
}

variable "moodle_web_service_user" {
  description = "Name of Moodle web service user"
  type        = string
  sensitive   = true
}

variable "moodle_web_service_user_email" {
  description = "Email of Moodle web service user"
  type        = string
  sensitive   = true
}

variable "basic_auth_user" {
  description = "User ID for basic auth protected sites"
  type        = string
  sensitive   = true
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
