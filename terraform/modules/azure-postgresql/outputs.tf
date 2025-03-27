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
