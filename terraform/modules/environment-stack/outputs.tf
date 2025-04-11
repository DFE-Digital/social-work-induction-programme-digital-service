output "resource_group_name" {
  description = "Name of the stack Resource Group"
  value       = azurerm_resource_group.rg_primary.name
}

output "vnet_id" {
  description = "ID of the Virtual Network"
  value       = azurerm_virtual_network.vnet_stack.id
}

output "vnet_name" {
  description = "Name of the Virtual Network"
  value       = azurerm_virtual_network.vnet_stack.name
}

output "kv_id" {
  description = "ID of the Key Vault"
  value       = azurerm_key_vault.kv.id
}

output "kv_vault_uri" {
  description = "URI of the key vault"
  value       = azurerm_key_vault.kv.vault_uri
}

output "db_server_id" {
  description = "The ID of the Postgresql database server"
  value       = azurerm_postgresql_flexible_server.swipdb.id
}

output "postgres_db_host" {
  description = "The postgresql database server"
  value       = azurerm_private_dns_a_record.dns_a_postgres.fqdn
}

output "postgres_username" {
  description = "The database admin username"
  value       = azurerm_postgresql_flexible_server.swipdb.administrator_login
}

output "full_postgres_secret_password_uri" {
  description = "The full Key Vault URI for the database password"
  value       = "${azurerm_key_vault.kv.vault_uri}secrets/${azurerm_key_vault_secret.database_password.name}"
}

output "front_door_profile_web_id" {
  description = "The ID of the Front Door profile"
  value = azurerm_cdn_frontdoor_profile.front_door_profile_web.id
}

output "subnet_webapps_id" {
  description = "The ID of the web apps subnet"
  value = azurerm_subnet.sn_webapps.id
}

output "service_plan_id" {
  description = "ID of the app service plan"
  value       = azurerm_service_plan.asp.id
}
