output "cert_name" {
  description = "Name of certificate created"
  value       = azurerm_key_vault_certificate.kv_cert.name
}
