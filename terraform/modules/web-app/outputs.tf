output "web_app_id" {
  description = "ID of the web app"
  value       = azurerm_linux_web_app.webapp.identity.0.principal_id
}

output "web_app_name" {
  description = "Name of the web app"
  value       = azurerm_linux_web_app.webapp.name
}

output "web_app_url" {
  description = "Default HTTPS URL of the Linux web app"
  value       = "https://${azurerm_linux_web_app.webapp.default_hostname}"
}

output "front_door_app_url" {
  description = "Front Door HTTPS URL of the Linux web app"
  value       = "https://${azurerm_cdn_frontdoor_endpoint.front_door_endpoint_web.host_name}"
}

output "front_door_endpoint_id" {
  description = "ID of the Front Door endpoint"
  value       = azurerm_cdn_frontdoor_endpoint.front_door_endpoint_web.id
}
