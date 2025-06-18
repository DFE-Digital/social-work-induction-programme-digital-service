resource "axurerm_linux_function_app" "function_app" {
  name                       = var.function_app_name
  location                   = var.location
  resource_group_name        = var.resource_group
  service_plan_id            = var.service_plan_id
  storage_account_name       = var.storage_account_name
  storage_account_access_key = var.storage_account_access_key

  identity {
    type = "SystemAssigned"
  }

  site_config = {
    # No application_stack needed, it's defined in the Docker image
  }

  app_settings = {
    docker_registry_url = "https://${var.acr_name}.azurecr.io"
    docker_image_name   = var.docker_image_name
  }

  lifecycle {
    ignore_changes = [
      tags["Environment"],
      tags["Product"],
      tags["Service Offering"],
      # The image tag will be updated by the CI/CD pipeline
      app_settings["DOCKER_CUSTOM_IMAGE_NAME"],
    ]
  }

  tags = var.tags
}

resource "azurerm_role_assignment" "acr_pull" {
  scope                = azurerm_container_registry.acr.id
  role_definition_name = "AcrPull"
  principal_id         = azurerm_linux_function_app.function_app.identity[0].principal_id
}
