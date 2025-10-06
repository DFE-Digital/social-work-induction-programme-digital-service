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
    always_on                         = true
    http2_enabled                     = true
    vnet_route_all_enabled            = true
    ftps_state                        = "Disabled"
    minimum_tls_version               = "1.3"
    health_check_path                 = var.health_check_path
    health_check_eviction_time_in_min = var.health_check_path == "" ? 2 : var.health_check_eviction_time_in_min

    ip_restriction_default_action = "Deny"

    dynamic "ip_restriction" {
      for_each = toset(var.allow_subnet_ids)
      content {
        name                      = "Allow from subnet"
        priority                  = 100
        action                    = "Allow"
        virtual_network_subnet_id = ip_restriction.value
      }
    }

    ip_restriction {
      name        = "Access from Front Door"
      priority    = 200
      action      = "Allow"
      service_tag = "AzureFrontDoor.Backend"
    }

    container_registry_use_managed_identity = true
    application_stack {
      docker_registry_url = "https://${var.acr_name}.azurecr.io"
      docker_image_name   = var.docker_image_name
    }
  }

  dynamic "storage_account" {
    for_each = var.storage_mounts

    content {
      # storage_account.key is the map key (e.g., "moodledata")
      # storage_account.value is the object with all the settings
      name         = storage_account.key
      type         = storage_account.value.type
      mount_path   = storage_account.value.mount_path
      account_name = storage_account.value.account_name
      share_name   = storage_account.value.share_name
      access_key   = storage_account.value.access_key
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

  # Provide standard setting which gives full app service domain name
  app_settings = merge({
    "FULL_EXTERNAL_WEB_DOMAIN_NAME" = "${var.web_app_name}.azurewebsites.net"
  }, var.app_settings)

  lifecycle {
    ignore_changes = [
      tags,
      # Ignore changes to the currently deployed image - CD will be changing this
      site_config.0.application_stack,
      # This is particularly sneaky. When the swift network connection is set later on, the
      # virtual_network_subnet_id is updated and the next time around, Terraform will reset
      # it back to null, removing the vnet / dbs integration. Then re-create it. 
      # Then set it to null...So the behaviour will alternate on each GA workflow run.
      # Hence we ignore any changes to virtual_network_subnet_id.
      virtual_network_subnet_id,
      logs
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
