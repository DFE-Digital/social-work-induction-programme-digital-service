resource "azurerm_app_service_virtual_network_swift_connection" "asvnsc_funcapp" {
  app_service_id = azurerm_linux_function_app.function_app.id
  subnet_id      = var.subnet_functionapp_id
}
