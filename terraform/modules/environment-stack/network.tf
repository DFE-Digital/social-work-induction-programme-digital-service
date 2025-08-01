resource "azurerm_virtual_network" "vnet_stack" {
  name                = "${var.resource_name_prefix}-vnet"
  location            = var.location
  resource_group_name = azurerm_resource_group.rg_primary.name
  address_space       = ["10.0.0.0/20"] # Gives us 10.0.0.0 → 10.0.15.255.

  tags = var.tags
}


resource "azurerm_network_security_group" "funcapp_nsg" {
  name                = "${var.resource_name_prefix}-nsg-funcapp"
  location            = var.location
  resource_group_name = azurerm_resource_group.rg_primary.name
  tags                = var.tags

  security_rule {
    name                       = "${var.resource_name_prefix}-allow-services"
    priority                   = 100
    direction                  = "Inbound"
    access                     = "Allow"
    protocol                   = "Tcp"
    source_port_range          = "*"
    destination_port_range     = "443"
    source_address_prefix      = azurerm_subnet.sn_service_apps.address_prefixes[0]
    destination_address_prefix = azurerm_subnet.sn_function_app.address_prefixes[0]
  }
}

resource "azurerm_subnet_network_security_group_association" "funcapp_nsg_assoc" {
  subnet_id                 = azurerm_subnet.sn_function_app.id
  network_security_group_id = azurerm_network_security_group.funcapp_nsg.id
}
