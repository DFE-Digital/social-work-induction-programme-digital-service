output "function_app_url" {
  description = "The HTTPS URL of the function app"
  value       = "https://${azurerm_linux_function_app.function_app.default_hostname}"
}
