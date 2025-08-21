resource "azurerm_container_app_job" "job" {
  name                         = var.container_app_job_name
  location                     = var.location
  resource_group_name          = var.resource_group_name
  container_app_environment_id = var.container_app_env_id

  replica_timeout_in_seconds = 120
  replica_retry_limit        = 10

  manual_trigger_config {
    parallelism              = 1
    replica_completion_count = 1
  }

  identity {
    type = "SystemAssigned"
  }

  #   registry {
  #     server = "https://${var.acr_name}.azurecr.io"
  #   }

  template {
    container {
      name   = "operations-container"
      image  = var.container_image
      cpu    = 0.5
      memory = "1Gi"

      dynamic "env" {
        for_each = var.environment_settings
        content {
          name  = env.key
          value = env.value
        }
      }
    }
  }

  tags = var.tags

  lifecycle {
    ignore_changes = [
      template.0.container[0].image,
      tags
    ]
  }
}

resource "azurerm_key_vault_access_policy" "kv_policy" {
  key_vault_id = var.key_vault_id
  tenant_id    = azurerm_container_app_job.job.identity.0.tenant_id
  object_id    = azurerm_container_app_job.job.identity.0.principal_id

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
