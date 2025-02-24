# Create Virtual Network
resource "azurerm_virtual_network" "vnet" {
  name                = "${var.resource_name_prefix}-vnet"
  location            = var.location
  resource_group_name = var.resource_group
  address_space       = ["10.0.0.0/22"]

  lifecycle {
    ignore_changes = [
      tags["Environment"],
      tags["Product"],
      tags["Service Offering"]
    ]
  }
}
