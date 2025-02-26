output "default_hostname" {
  description = "Default name of the app service host"
  value       = azurerm_linux_web_app.webapp.default_hostname
}
