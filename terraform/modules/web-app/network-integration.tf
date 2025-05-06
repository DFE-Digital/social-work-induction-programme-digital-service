resource "azurerm_app_service_virtual_network_swift_connection" "asvnsc_webapp" {
  app_service_id = azurerm_linux_web_app.webapp.id
  subnet_id      = var.subnet_webapps_id
}

resource "azurerm_key_vault_access_policy" "kv_policy" {
  key_vault_id = var.key_vault_id
  tenant_id    = var.tenant_id
  object_id    = azurerm_linux_web_app.webapp.identity.0.principal_id

  key_permissions = [
    "Get",
    "List",
    "UnwrapKey"
  ]

  secret_permissions = [
    "Get",
    "List",
  ]

  certificate_permissions = [
    "Get",
    "List",
  ]
}

resource "azurerm_role_assignment" "acr_role" {
  scope                = var.acr_id
  role_definition_name = "AcrPull"
  principal_type       = "ServicePrincipal"
  principal_id         = azurerm_linux_web_app.webapp.identity.0.principal_id
}
