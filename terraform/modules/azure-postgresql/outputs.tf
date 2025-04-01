output "postgres_subnet_id" {
  description = "The ID of the Postgresql subnet"
  value       = azurerm_subnet.postgres_sn.id  
}

output "postgres_private_dns_zone_id" {
  description = "The ID of the Postgresql private DNS zone"
  value       = azurerm_private_dns_zone.private_dns.id
}

output "postgres_secret_uri" {
  description = "The Key Vault entry for the database password"
  value       = azurerm_key_vault_secret.database_password.name
}

output "postgres_db_host" {
  description = "The postgresql database server"
  value       = azurerm_private_dns_zone.private_dns.name
}

output "postgres_username" {
  description = "The database admin username"
  value       = azurerm_postgresql_flexible_server.swipdb.administrator_login
}
