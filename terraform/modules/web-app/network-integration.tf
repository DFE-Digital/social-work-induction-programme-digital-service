resource "azurerm_app_service_virtual_network_swift_connection" "asvnsc_webapp" {
  app_service_id = azurerm_linux_web_app.webapp.id
  subnet_id      = var.subnet_webapps_id
}
