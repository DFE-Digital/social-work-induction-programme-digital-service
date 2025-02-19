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
    "ServiceAccess__IsPublic"             = var.webapp_access_is_public
    "ServiceAccess__Keys__0"              = var.webapp_e2e_access_key
    "ServiceAccess__Keys__1"              = var.webapp_team_access_key
    "ServiceAccess__Keys__2"              = var.webapp_access_key_1
    "ServiceAccess__Keys__3"              = var.webapp_access_key_2
    "GTM__Tag"                            = var.gtm_tag
    "Clarity__Tag"                        = var.clarity_tag
  }

  webapp_slot_app_settings = {
    "ENVIRONMENT"                         = var.environment
    "WEBSITES_ENABLE_APP_SERVICE_STORAGE" = "false"
    "WEBSITES_CONTAINER_START_TIME_LIMIT" = 720
    "KeyVault__Endpoint"                  = "https://${var.resource_name_prefix}-kv.vault.azure.net/"
    "WEBSITES_PORT"                       = "8080"
    "ServiceAccess__IsPublic"             = var.webapp_access_is_public
    "ServiceAccess__Keys__0"              = var.webapp_e2e_access_key
    "ServiceAccess__Keys__1"              = var.webapp_team_access_key
    "ServiceAccess__Keys__2"              = var.webapp_access_key_1
    "ServiceAccess__Keys__3"              = var.webapp_access_key_2
  }
}
