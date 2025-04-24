output "web_app_name" {
  description = "Name of the web app"
  value       = azurerm_linux_web_app.webapp.name
}

output "web_app_url" {
  description = "Default HTTPS URL of the Linux web app"
  value       = "https://${azurerm_linux_web_app.webapp.default_hostname}"
}
