resource "azurerm_virtual_network" "vnet_stack" {
  name                = "${var.resource_name_prefix}-vnet"
  location            = var.location
  resource_group_name = azurerm_resource_group.rg_primary.name
  address_space       = ["10.0.0.0/20"] # Gives us 10.0.0.0 → 10.0.15.255.

  tags = var.tags
}
