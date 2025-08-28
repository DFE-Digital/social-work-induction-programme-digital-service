resource "azurerm_app_service_virtual_network_swift_connection" "asvnsc_functionapp" {
  app_service_id = azurerm_linux_function_app.function_app.id
  subnet_id      = var.subnet_functionapp_id
}

# resource "azurerm_network_security_group" "private_endpoint_nsg" {
#   name                = "${var.function_app_name}-pe-nsg"
#   location            = var.location
#   resource_group_name = var.resource_group

#   security_rule {
#     name                       = "AllowVnetInbound"
#     priority                   = 100
#     direction                  = "Inbound"
#     access                     = "Allow"
#     protocol                   = "*"
#     source_port_range          = "*"
#     destination_port_range     = "*"
#     source_address_prefix      = "VirtualNetwork"
#     destination_address_prefix = "VirtualNetwork"
#   }

#   security_rule {
#     name                       = "AllowAzureLoadBalancerInbound"
#     priority                   = 110
#     direction                  = "Inbound"
#     access                     = "Allow"
#     protocol                   = "Tcp"
#     source_port_range          = "*"
#     destination_port_range     = "*"
#     source_address_prefix      = "AzureLoadBalancer"
#     destination_address_prefix = "*"
#   }
# }

# Move
# resource "azurerm_subnet" "private_endpoint" {
#   name                 = "endpoint-subnet"
#   resource_group_name  = var.resource_group
#   virtual_network_name = var.virtual_network_name
#   address_prefixes     = ["10.0.7.0/24"]

#   # This setting is required for private endpoint subnets.
#   private_endpoint_network_policies = "Disabled"
# }

# resource "azurerm_subnet_network_security_group_association" "private_endpoint_nsg_association" {
#   subnet_id                 = azurerm_subnet.private_endpoint.id
#   network_security_group_id = azurerm_network_security_group.private_endpoint_nsg.id
# }

# resource "azurerm_private_dns_zone" "pvt_dns_zone" {
#   name                = "privatelink.azurewebsites.net"
#   resource_group_name = var.resource_group
# }

# resource "azurerm_private_dns_zone_virtual_network_link" "vnet_link" {
#   name                  = "${var.virtual_network_name}-dns-link"
#   resource_group_name   = var.resource_group
#   private_dns_zone_name = azurerm_private_dns_zone.pvt_dns_zone.name
#   virtual_network_id    = var.virtual_network_id
# }

# Keep this here
resource "azurerm_private_endpoint" "function_app_endpoint" {
  name                = "${azurerm_linux_function_app.function_app.name}-endpoint"
  location            = var.location
  resource_group_name = var.resource_group
  subnet_id           = var.subnet_functionapp_privateendpoint_id

  # Create the necessary DNS 'A' record.
  private_dns_zone_group {
    name                 = "default"
    private_dns_zone_ids = [var.private_dns_zone_id]
  }

  # Connect the endpoint to the Function App.
  private_service_connection {
    name                           = "${azurerm_linux_function_app.function_app.name}-connection"
    is_manual_connection           = false
    private_connection_resource_id = azurerm_linux_function_app.function_app.id
    subresource_names              = ["sites"]
  }
}
