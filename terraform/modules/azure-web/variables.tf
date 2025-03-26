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

variable "asp_sku" {
  description = "SKU name for the App Service Plan"
  type        = string
}

variable "webapp_worker_count" {
  description = "Number of Workers for the App Service Plan"
  type        = string
}

variable "webapp_name" {
  description = "Name for the Web Application"
  type        = string
}

variable "tags" {
  description = "Resource tags"
  type        = map(string)
}

variable "kv_id" {
  description = "ID of the Key Vault"
  type        = string
}

variable "moodle_db_name" {
  description = "The name of the postgres database"
  type        = string
}

variable "postgres_username" {
  description = "The username for the Moodle Postgresql database"
  type        = string
}

variable "postgres_secret_uri" {
  description = "The Key Vault secret containing the password for the the Postgresql database"
  type        = string
}

variable "moodle_db_type" {
  description = "The database type for Moodle"
  type        = string
  default     = "pgsql"
}

variable "moodle_db_host" {
  description = "The hostname for the Postgresql database"
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
  default     = "80"
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

variable "moodle_admin_email" {
  description = "The email address to use for the admin user on Moodle"
  type        = string
}

variable "moodle_admin_password" {
  description = "The password for Moodle admin user"
  type        = string
  sensitive   = true
}

variable "acr_id" {
  description = "Resource ID of the ACR"
  type        = string
}
