resource "azurerm_app_service_virtual_network_swift_connection" "asvnsc_functionapp" {
  app_service_id = azurerm_linux_function_app.function_app.id
  subnet_id      = var.subnet_functionapp_id
}

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
