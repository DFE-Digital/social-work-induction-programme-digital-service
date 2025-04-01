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

variable "moodle_db_type" {
  description = "The database type for Moodle"
  type        = string
  default     = "pgsql"
}

variable "moodle_db_name" {
  description = "The name of the database"
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
}

variable "moodle_admin_password" {
  description = "The password for Moodle admin user"
  type        = string
  sensitive   = true
}

variable "moodle_admin_email" {
  description = "The email address to use for the admin user on Moodle"
  type        = string
}
