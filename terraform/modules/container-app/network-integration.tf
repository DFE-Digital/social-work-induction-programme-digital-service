resource "azurerm_role_assignment" "acr_role" {
  scope                = var.acr_id
  role_definition_name = "AcrPull"
  principal_type       = "ServicePrincipal"
  principal_id         = azurerm_container_app_job.job.identity.0.principal_id
}
