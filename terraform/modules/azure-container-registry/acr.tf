resource "azurerm_container_registry" "acr" {
  name                          = "${var.resource_name_prefix}-acr"
  resource_group_name           = var.resource_group
  location                      = var.location
  sku                           = var.acr_sku
  admin_enabled                 = var.admin_enabled
  public_network_access_enabled = var.public_network_access_enabled
  tags                          = var.tags
}
