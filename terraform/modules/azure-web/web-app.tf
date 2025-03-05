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

resource "azurerm_user_assigned_identity" "webapp_identity" {
  name                = "webapp-identity"
  resource_group_name = var.resource_group
  location            = var.location
}

resource "azurerm_role_assignment" "acr_pull_webapp" {
  scope                = var.acr_id
  role_definition_name = "AcrPull"
  principal_id         = azurerm_user_assigned_identity.webapp_identity.principal_id
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
    type         = "UserAssigned"
    identity_ids = [azurerm_user_assigned_identity.webapp_identity.id]
  }

  site_config {
    always_on              = true
    http2_enabled          = true
    vnet_route_all_enabled = true
    ftps_state             = "Disabled"
    minimum_tls_version    = "1.2"

    ip_restriction_default_action = "Deny"

    ip_restriction {
      name        = "Access from Front Door"
      service_tag = "AzureFrontDoor.Backend"
    }

    health_check_path                 = "/health"
    health_check_eviction_time_in_min = 5

    linux_fx_version = "DOCKER|${var.webapp_docker_registry_url}/${var.webapp_docker_image}:${var.webapp_docker_image_tag}"
  }

  sticky_settings {
    app_setting_names = keys(var.webapp_app_settings)
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
