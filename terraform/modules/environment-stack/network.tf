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

resource "azurerm_network_security_group" "private_endpoint_nsg" {
  name                = "${var.resource_name_prefix}-pe-nsg"
  location            = var.location
  resource_group_name = azurerm_resource_group.rg_primary.name
  tags                = var.tags

  security_rule {
    name                       = "AllowVnetInbound"
    priority                   = 100
    direction                  = "Inbound"
    access                     = "Allow"
    protocol                   = "*"
    source_port_range          = "*"
    destination_port_range     = "*"
    source_address_prefix      = "VirtualNetwork"
    destination_address_prefix = "VirtualNetwork"
  }

  security_rule {
    name                       = "AllowAzureLoadBalancerInbound"
    priority                   = 110
    direction                  = "Inbound"
    access                     = "Allow"
    protocol                   = "Tcp"
    source_port_range          = "*"
    destination_port_range     = "*"
    source_address_prefix      = "AzureLoadBalancer"
    destination_address_prefix = "*"
  }

  lifecycle {
    ignore_changes = [tags]
  }
}

resource "azurerm_subnet_network_security_group_association" "funcapp_nsg_assoc" {
  subnet_id                 = azurerm_subnet.sn_function_app.id
  network_security_group_id = azurerm_network_security_group.funcapp_nsg.id
}

resource "azurerm_subnet" "private_endpoint" {
  name                 = "${var.resource_name_prefix}-sn-funcapp-endpoint"
  resource_group_name  = azurerm_resource_group.rg_primary.name
  virtual_network_name = azurerm_virtual_network.vnet_stack.name
  address_prefixes     = ["10.0.7.0/24"]

  # This setting is required for private endpoint subnets.
  private_endpoint_network_policies = "Disabled"
}

resource "azurerm_subnet_network_security_group_association" "private_endpoint_nsg_association" {
  subnet_id                 = azurerm_subnet.private_endpoint.id
  network_security_group_id = azurerm_network_security_group.private_endpoint_nsg.id
}

resource "azurerm_private_dns_zone" "pvt_dns_zone" {
  name                = "privatelink.azurewebsites.net"
  resource_group_name = azurerm_resource_group.rg_primary.name
  tags                = var.tags

  lifecycle {
    ignore_changes = [tags]
  }
}

resource "azurerm_private_dns_zone_virtual_network_link" "vnet_link" {
  name                  = "${azurerm_virtual_network.vnet_stack.name}-dns-link"
  resource_group_name   = azurerm_resource_group.rg_primary.name
  private_dns_zone_name = azurerm_private_dns_zone.pvt_dns_zone.name
  virtual_network_id    = azurerm_virtual_network.vnet_stack.id
  tags                  = var.tags

  lifecycle {
    ignore_changes = [tags]
  }
}

resource "azurerm_private_dns_zone" "pvt_storage_dns_zone" {
  name                = "privatelink.file.core.windows.net"
  resource_group_name = azurerm_resource_group.rg_primary.name
  tags                = var.tags

  lifecycle {
    ignore_changes = [tags]
  }
}

resource "azurerm_private_endpoint" "moodle_data_private_endpoint" {
  name                = "${azurerm_storage_account.sa_moodle_data.name}-pe"
  resource_group_name = azurerm_resource_group.rg_primary.name
  location            = var.location
  subnet_id           = azurerm_subnet.private_endpoint.id

  private_dns_zone_group {
    name                 = "default"
    private_dns_zone_ids = [azurerm_private_dns_zone.pvt_storage_dns_zone.id]
  }

  private_service_connection {
    name                           = "${azurerm_storage_account.sa_moodle_data.name}-psc"
    is_manual_connection           = false
    private_connection_resource_id = azurerm_storage_account.sa_moodle_data.id
    subresource_names              = ["file"]
  }
}
