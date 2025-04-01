resource "azurerm_resource_group" "rg" {
  name     = "${var.resource_name_prefix}-rg-swip"
  location = var.location
  tags     = var.tags

  lifecycle {
    ignore_changes = [tags]
  }
}