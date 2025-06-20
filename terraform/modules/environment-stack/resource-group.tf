data "azurerm_client_config" "az_config" {}

resource "azurerm_resource_group" "rg_primary" {
  name       = var.primary_resource_group
  location   = var.location
  managed_by = data.azurerm_client_config.az_config.object_id
  tags       = var.tags

  lifecycle {
    ignore_changes = [tags]
  }
}

# Assign ownership of the resource group to the Terraform service principal. Without this, 
# Terraform is not able to perform more advanced role assignments such as assigning the
# "Storage File Data SMB Share Contributor" role to a web app managed identity.

# resource "azurerm_role_assignment" "tf_sp_owner_on_rg" {
#   scope                = azurerm_resource_group.rg_primary.id
#   role_definition_name = "Owner"
#   principal_type       = "ServicePrincipal"
#   principal_id         = data.azurerm_client_config.az_config.object_id
# }
