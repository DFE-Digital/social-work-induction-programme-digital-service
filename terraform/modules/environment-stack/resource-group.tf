data "azurerm_client_config" "az_config" {}

resource "azurerm_resource_group" "rg_primary" {
  name       = var.primary_resource_group
  location   = var.location
  managed_by = data.azurerm_client_config.az_config.object_id
  tags       = var.tags

  lifecycle {
    ignore_changes = [tags]
  }
}
