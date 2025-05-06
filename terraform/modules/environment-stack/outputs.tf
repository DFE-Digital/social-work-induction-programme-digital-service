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

output "subnet_moodle_id" {
  description = "The ID of the Moodle subnet"
  value = azurerm_subnet.sn_moodle_app.id
}

output "subnet_maintenance_id" {
  description = "The ID of the maintenance apps subnet"
  value = azurerm_subnet.sn_maintenance_apps.id
}

output "subnet_services_id" {
  description = "The ID of the service apps subnet"
  value = azurerm_subnet.sn_service_apps.id
}

output "moodle_service_plan_id" {
  description = "ID of the Moodle app service plan"
  value       = azurerm_service_plan.asp_moodle_app.id
}

output "maintenance_service_plan_id" {
  description = "ID of the maintenance service plan"
  value       = azurerm_service_plan.asp_maintenance_apps.id
}

output "services_service_plan_id" {
  description = "ID of the service apps service plan"
  value       = azurerm_service_plan.asp_service_apps.id
}

output "storage_account_id" {
  description = "ID of the shared storage account"
  value       = azurerm_storage_account.sa.id
}

output "storage_account_name" {
  description = "Name of the shared storage account"
  value       = azurerm_storage_account.sa.name
}
