output "acr_id" {
  description = "ID of the ACR"
  value       = azurerm_container_registry.acr.id
}

output "acr_name" {
  description = "Name of the ACR"
  value       = azurerm_container_registry.acr.name
}
