resource "azurerm_resource_group" "rg_primary" {
  name     = var.primary_resource_group
  location = var.location
  tags     = var.tags

  lifecycle {
    ignore_changes = [tags]
  }
}
