resource "azurerm_linux_web_app" "webapp" {
  name                = var.web_app_name
  location            = var.location
  resource_group_name = var.resource_group
  service_plan_id     = var.service_plan_id
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
    application_stack {
      // TODO: Parameterize this
      docker_registry_url = "https://${var.resource_name_prefix}acr.azurecr.io"
      // TODO: Proper version management
      docker_image_name   = "dfe-digital/social-work-induction-programme-digital-service:latest"
    }
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

  app_settings = var.app_settings

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