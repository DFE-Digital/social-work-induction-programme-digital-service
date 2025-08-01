output "function_app_url" {
  description = "The HTTPS URL of the function app"
  value       = "https://${azurerm_linux_function_app.function_app.default_hostname}"
}

output "function_app_private_url" {
  description = "The private endpoint URL of the function app"
  value       = "https://${azurerm_private_endpoint.function_app_endpoint.private_dns_zone_configs[0].record_sets[0].fqdn}"
}

output "function_app_key" {
  description = "The default function key for the function app"
  value       = data.azurerm_function_app_host_keys.function_keys.default_function_key
}
