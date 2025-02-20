locals {
  # Common tags to be assigned to resources
  common_tags = {
    "Environment"      = var.environment
    "Parent Business"  = "Children and Families"
    "Product"          = "Social Work Induction Programme"
    "Service Offering" = "Social Work Induction Programme"
  }

  # Web Application Configuration
  webapp_app_settings = {
    "ENVIRONMENT"                         = var.environment
    "WEBSITES_ENABLE_APP_SERVICE_STORAGE" = "false"
    "WEBSITES_CONTAINER_START_TIME_LIMIT" = 720
    "KeyVault__Endpoint"                  = "https://${var.resource_name_prefix}-kv.vault.azure.net/"
    "WEBSITES_PORT"                       = "8080"
  }

  webapp_slot_app_settings = {
    "ENVIRONMENT"                         = var.environment
    "WEBSITES_ENABLE_APP_SERVICE_STORAGE" = "false"
    "WEBSITES_CONTAINER_START_TIME_LIMIT" = 720
    "KeyVault__Endpoint"                  = "https://${var.resource_name_prefix}-kv.vault.azure.net/"
    "WEBSITES_PORT"                       = "8080"
  }
}
