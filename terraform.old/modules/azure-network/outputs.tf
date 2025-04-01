output "vnet_id" {
  description = "ID of the Virtual Network"
  value       = azurerm_virtual_network.vnet.id
}

output "vnet_name" {
  description = "Name of the Virtual Network"
  value       = azurerm_virtual_network.vnet.name
}

output "kv_id" {
  description = "ID of the Key Vault"
  value       = azurerm_key_vault.kv.id
}

output "kv_vault_uri" {
  description = "URI of the key vault"
  value       = azurerm_key_vault.kv.vault_uri
}
