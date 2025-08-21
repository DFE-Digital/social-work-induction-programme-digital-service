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
  value       = azurerm_cdn_frontdoor_profile.front_door_profile_web.id
}

output "subnet_moodle_id" {
  description = "The ID of the Moodle subnet"
  value       = azurerm_subnet.sn_moodle_app.id
}

output "subnet_maintenance_id" {
  description = "The ID of the maintenance apps subnet"
  value       = azurerm_subnet.sn_maintenance_apps.id
}

output "subnet_services_id" {
  description = "The ID of the service apps subnet"
  value       = azurerm_subnet.sn_service_apps.id
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

output "blob_storage_account_id" {
  description = "ID of the shared blob storage account"
  value       = azurerm_storage_account.sa_app_blob_storage.id
}

output "blob_storage_account_name" {
  description = "Name of the shared blob storage account"
  value       = azurerm_storage_account.sa_app_blob_storage.name
}

output "file_storage_account_id" {
  description = "ID of the shared file storage account"
  value       = azurerm_storage_account.sa_app_file_storage.id
}

output "file_storage_account_name" {
  description = "Name of the shared file storage account"
  value       = azurerm_storage_account.sa_app_file_storage.name
}

output "file_storage_access_key" {
  description = "Primary access key for the shared file storage account"
  value       = azurerm_storage_account.sa_app_file_storage.primary_access_key
  sensitive   = true
}

output "support_action_group_id" {
  description = "ID of the support action group"
  value       = azurerm_monitor_action_group.stack_action_group.id
}

output "notification_service_plan_id" {
  description = "ID of the notification app service plan"
  value       = azurerm_service_plan.asp_notification_service.id
}

output "appinsights_connection_string" {
  description = "Connection string to Application Insights"
  value       = azurerm_application_insights.app_insights_web.connection_string
}

output "subnet_functionapp_id" {
  description = "The ID of the function app subnet"
  value       = azurerm_subnet.sn_function_app.id
}

output "subnet_containerapps_id" {
  description = "The ID of the container apps subnet"
  value       = azurerm_subnet.cae_subnet.id
}

output "db_backup_blob_Storage_account_id" {
  description = "The ID of the blob storage account for DB backups"
  value       = azurerm_storage_account.sa_db_backup_blob_storage.id
}

output "db_backup_blob_storage_account_name" {
  description = "The name of the blob storage account"
  value       = azurerm_storage_account.sa_db_backup_blob_storage.name
}

output "log_analytics_workspace_id" {
  description = "The ID of the Log Analytics Workspace"
  value       = azurerm_log_analytics_workspace.log_analytics_web.id
}

output "container_env_id" {
  description = "The ID of the Container App Environment"
  value       = azurerm_container_app_environment.env.id
}
