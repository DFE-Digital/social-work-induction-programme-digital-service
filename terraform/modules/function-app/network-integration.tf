resource "azurerm_app_service_virtual_network_swift_connection" "asvnsc_functionapp" {
  app_service_id = azurerm_linux_function_app.function_app.id
  subnet_id      = var.subnet_functionapp_id
}

resource "azurerm_subnet" "private_endpoint" {
  name                 = "endpoint-subnet"
  resource_group_name  = var.resource_group
  virtual_network_name = var.virtual_network_name
  address_prefixes     = ["10.0.7.0/24"]

  # This setting is required for private endpoint subnets.
  private_endpoint_network_policies = "Disabled"
}

resource "azurerm_private_dns_zone" "pvt_dns_zone" {
  name                = "privatelink.azurewebsites.net"
  resource_group_name = var.resource_group
}

resource "azurerm_private_dns_zone_virtual_network_link" "vnet_link" {
  name                  = "${azurerm_virtual_network.vnet_stack.name}-dns-link"
  resource_group_name   = var.resource_group
  private_dns_zone_name = azurerm_private_dns_zone.pvt_dns_zone.name
  virtual_network_id    = var.virtual_network_id
}

resource "azurerm_private_endpoint" "function_app_endpoint" {
  name                = "${azurerm_linux_function_app.function_app.name}-endpoint"
  location            = var.location
  resource_group_name = var.resource_group
  subnet_id           = var.subnet_functionapp_id

  # Create the necessary DNS 'A' record.
  private_dns_zone_group {
    name                 = "default"
    private_dns_zone_ids = [azurerm_private_dns_zone.pvt_dns_zone.id]
  }

  # Connect the endpoint to the Function App.
  private_service_connection {
    name                           = "${azurerm_linux_function_app.function_app.name}-connection"
    is_manual_connection           = false
    private_connection_resource_id = azurerm_linux_function_app.function_app.id
    subresource_names              = ["sites"]
  }
}
