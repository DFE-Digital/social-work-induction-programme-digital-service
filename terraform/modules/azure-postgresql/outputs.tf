output "azurerm_postgresql_flexible_server" {
  value = azurerm_postgresql_flexible_server.swipdb.name
}

output "postgresql_flexible_server_database_name" {
  value = azurerm_postgresql_flexible_server_database.swipdb.name
}

output "postgresql_flexible_server_admin_password" {
  sensitive = true
  value     = azurerm_postgresql_flexible_server.swipdb.administrator_password
}
