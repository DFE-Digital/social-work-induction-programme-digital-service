resource "azurerm_linux_function_app" "function_app" {
  name                          = var.function_app_name
  location                      = var.location
  resource_group_name           = var.resource_group
  service_plan_id               = var.service_plan_id
  storage_account_name          = var.storage_account_name
  storage_account_access_key    = var.storage_account_access_key
  https_only                    = true
  public_network_access_enabled = false

  identity {
    type = "SystemAssigned"
  }

  site_config {
    # No application_stack needed, it's defined in the Docker image
    always_on                               = true
    health_check_path                       = var.health_check_path
    health_check_eviction_time_in_min       = var.health_check_path == "" ? 2 : var.health_check_eviction_time_in_min
    vnet_route_all_enabled                  = true
    container_registry_use_managed_identity = true
    application_stack {
      docker {
        image_name   = var.docker_image_name
        image_tag    = "latest"
        registry_url = "https://${var.acr_name}.azurecr.io"
      }
    }
    app_service_logs {
      disk_quota_mb         = 25
      retention_period_days = 1
    }

  }

  app_settings = merge({
    "DOCKER_REGISTRY_SERVER_URL"            = "https://${var.acr_name}.azurecr.io"
    "APPLICATIONINSIGHTS_CONNECTION_STRING" = var.appinsights_connection_string
    "FUNCTIONS_WORKER_RUNTIME"              = var.function_worker_runtime
    "WEBSITES_ENABLE_APP_SERVICE_STORAGE"   = "false"
    "DOCKER_REGISTRY_SERVER_USERNAME"       = ""
    "DOCKER_REGISTRY_SERVER_PASSWORD"       = ""
    DOCKER_ENABLE_CI                        = "false" # Github will control CI, not Azure
  }, var.app_settings)

  tags = var.tags

  lifecycle {
    ignore_changes = [
      tags["Environment"],
      tags["Product"],
      tags["Service Offering"],
      # Ignore changes to the currently deployed image - CD will be changing this
      site_config.0.application_stack,
      # This is particularly sneaky. When the swift network connection is set later on, the
      # virtual_network_subnet_id is updated and the next time around, Terraform will reset
      # it back to null, removing the vnet / dbs integration. Then re-create it. 
      # Then set it to null...So the behaviour will alternate on each GA workflow run.
      # Hence we ignore any changes to virtual_network_subnet_id.
      virtual_network_subnet_id
    ]
  }
}

resource "azurerm_role_assignment" "acr_pull" {
  scope                = var.acr_id
  role_definition_name = "AcrPull"
  principal_type       = "ServicePrincipal"
  principal_id         = azurerm_linux_function_app.function_app.identity.0.principal_id
}

resource "azurerm_key_vault_access_policy" "kv_policy" {
  key_vault_id = var.key_vault_id
  tenant_id    = azurerm_linux_function_app.function_app.identity.0.tenant_id
  object_id    = azurerm_linux_function_app.function_app.identity.0.principal_id

  key_permissions = [
    "Get",
    "List",
    "UnwrapKey"
  ]

  secret_permissions = [
    "Get",
    "List",
  ]

  certificate_permissions = [
    "Get",
    "List",
  ]
}
