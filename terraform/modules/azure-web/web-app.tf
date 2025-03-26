# Create Log Analytics
resource "azurerm_log_analytics_workspace" "webapp_logs" {
  name                = "${var.resource_name_prefix}-log"
  location            = var.location
  resource_group_name = var.resource_group
  sku                 = "PerGB2018"
  retention_in_days   = 30
  daily_quota_gb      = 1

  lifecycle {
    ignore_changes = [
      tags["Environment"],
      tags["Product"],
      tags["Service Offering"]
    ]
  }
}

resource "azurerm_application_insights" "web" {
  name                = "${var.resource_name_prefix}-appinsights"
  resource_group_name = var.resource_group
  location            = var.location
  application_type    = "web"
  workspace_id        = azurerm_log_analytics_workspace.webapp_logs.id
  tags                = var.tags

  lifecycle {
    ignore_changes = [
      tags["Environment"],
      tags["Product"],
      tags["Service Offering"]
    ]
  }
}

# Create App Service Plan
resource "azurerm_service_plan" "asp" {
  name                = "${var.resource_name_prefix}-asp"
  location            = var.location
  resource_group_name = var.resource_group
  os_type             = "Linux"
  sku_name            = var.asp_sku
  worker_count        = var.webapp_worker_count

  lifecycle {
    ignore_changes = [
      tags["Environment"],
      tags["Product"],
      tags["Service Offering"]
    ]
  }

  #checkov:skip=CKV_AZURE_212:Argument not available
  #checkov:skip=CKV_AZURE_225:Ensure the App Service Plan is zone redundant
}

# Create Web Application
resource "azurerm_linux_web_app" "webapp" {
  name                = var.webapp_name
  location            = var.location
  resource_group_name = var.resource_group
  service_plan_id     = azurerm_service_plan.asp.id
  https_only          = true
  identity {
    type = "SystemAssigned"
  }

  site_config {
    always_on              = true
    http2_enabled          = true
    vnet_route_all_enabled = true
    ftps_state             = "Disabled"
    minimum_tls_version    = "1.3"

    ip_restriction_default_action = "Deny"

    ip_restriction {
      name        = "Access from Front Door"
      service_tag = "AzureFrontDoor.Backend"
    }

    container_registry_use_managed_identity = true
  }

  logs {
    detailed_error_messages = true
    failed_request_tracing  = true

    application_logs {
      file_system_level = "Warning"
    }

    http_logs {
      file_system {
        retention_in_days = 1
        retention_in_mb   = 25
      }
    }
  }

  app_settings = {
    "ENVIRONMENT"                         = var.environment
    "WEBSITES_ENABLE_APP_SERVICE_STORAGE" = "false"
    "POSTGRES_DB"                         = var.moodle_db_name
    "POSTGRES_USER"                       = var.postgres_username
    "POSTGRES_PASSWORD"                   = "@Microsoft.KeyVault(SecretUri=${var.postgres_secret_uri})"
    "MOODLE_DB_TYPE"                      = var.moodle_db_type
    "MOODLE_DB_HOST"                      = var.moodle_db_host
    "MOODLE_DB_PREFIX"                    = var.moodle_db_prefix
    "MOODLE_DOCKER_WEB_HOST"              = var.webapp_name
    "MOODLE_DOCKER_WEB_PORT"              = var.moodle_web_port
    "MOODLE_SITE_FULLNAME"                = var.moodle_site_fullname
    "MOODLE_SITE_SHORTNAME"               = var.moodle_site_shortname
    "MOODLE_ADMIN_USER"                   = var.moodle_admin_user
    "MOODLE_ADMIN_PASSWORD"               = var.moodle_admin_password
    "MOODLE_ADMIN_EMAIL"                  = var.moodle_admin_email
  }

  lifecycle {
    ignore_changes = [
      tags["Environment"],
      tags["Product"],
      tags["Service Offering"],
      site_config.0.application_stack
    ]
  }

  tags = var.tags

  #checkov:skip=CKV_AZURE_13:App uses built-in authentication
  #checkov:skip=CKV_AZURE_88:Using Docker
  #checkov:skip=CKV_AZURE_17:Argument not available
  #checkov:skip=CKV_AZURE_78:Disabled by default in Terraform version used
  #checkov:skip=CKV_AZURE_16:Using VNET Integration
  #checkov:skip=CKV_AZURE_71:Using VNET Integration
  #checkov:skip=CKV_AZURE_222:Network access rules configured
  #checkov:skip=CKV_AZURE_213:Ensure that App Service configures health check
}

resource "azurerm_monitor_diagnostic_setting" "webapp_logs_monitor" {

  name                       = "${var.resource_name_prefix}-webapp-mon"
  target_resource_id         = azurerm_linux_web_app.webapp.id
  log_analytics_workspace_id = azurerm_log_analytics_workspace.webapp_logs.id

  enabled_log {
    category = "AppServiceConsoleLogs"
  }

  enabled_log {
    category = "AppServicePlatformLogs"
  }

  timeouts {
    read = "30m"
  }

  lifecycle {
    ignore_changes = [metric]
  }
}

data "azurerm_client_config" "az_config" {}

# References the web app to be used in KV access policy as it already existed when changes needed to be made
data "azurerm_linux_web_app" "ref" {
  name                = azurerm_linux_web_app.webapp.name
  resource_group_name = azurerm_linux_web_app.webapp.resource_group_name
}

# Grants permissions to key vault for the managed identity of the App Service
resource "azurerm_key_vault_access_policy" "webapp_kv_app_service" {
  key_vault_id            = var.kv_id
  tenant_id               = data.azurerm_client_config.az_config.tenant_id
  object_id               = data.azurerm_linux_web_app.ref.identity.0.principal_id
  key_permissions         = ["Get", "UnwrapKey", "WrapKey"]
  secret_permissions      = ["Get", "List"]
  certificate_permissions = ["Get"]

  lifecycle {
    ignore_changes = [object_id, tenant_id]
  }
}

# Grants permission for the managed identity to pull images from ACR
resource "azurerm_role_assignment" "acr_role" {
  scope                = var.acr_id
  role_definition_name = "AcrPull"
  principal_type       = "ServicePrincipal"
  principal_id         = azurerm_linux_web_app.webapp.identity[0].principal_id
}
