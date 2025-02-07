terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "4.18.0"
    }
  }

  backend "azurerm" {
    resource_group_name  = "s205d01-swip-rg" # Can be passed via `-backend-config=`"resource_group_name=<resource group name>"` in the `init` command.
    storage_account_name = "s205d01vmdiagnosticss"                     # Can be passed via `-backend-config=`"storage_account_name=<storage account name>"` in the `init` command.
    container_name       = "tfstate"                      # Can be passed via `-backend-config=`"container_name=<container name>"` in the `init` command.
    key                  = "terraform.tfstate"       # Can be passed via `-backend-config=`"key=<blob key name>"` in the `init` command.
    use_oidc             = false                           # Can also be set via `ARM_USE_OIDC` environment variable.
    client_id            = "57be8663-a606-4669-97fa-1d1b9afdc714"
    tenant_id            = "9c7d9dd3-840c-4b3f-818e-552865082e16"
    client_secret        = "Pk~8Q~iSrogLJoHbBV6L1YreVRmEctdmpcculb2e"
    subscription_id      = "4d64147b-6de9-4dc7-ada7-6882ba4953e0"
  }
} 


provider "azurerm" {
  features {}
   # client_secret   = "Pk~8Q~iSrogLJoHbBV6L1YreVRmEctdmpcculb2e"
    client_id            = "57be8663-a606-4669-97fa-1d1b9afdc714"
    tenant_id            = "9c7d9dd3-840c-4b3f-818e-552865082e16"
    client_secret        = "Pk~8Q~iSrogLJoHbBV6L1YreVRmEctdmpcculb2e"
     subscription_id      = "4d64147b-6de9-4dc7-ada7-6882ba4953e0"
} 


resource "azurerm_resource_group" "example" {
  name     = "s205d01-swip"
  location = "West Europe"
  tags = {
    Service : "Early career framework for social work"
    Product : "Early career framework for social work"
    Portfolio : "Children and Families"
    Environment : "Dev"
  }
}

resource "azurerm_storage_account" "mystorage" {
  name                     = "s205d01vmdiagnostics"
  resource_group_name      = azurerm_resource_group.example.name
  location                 = azurerm_resource_group.example.location
  account_tier             = "Standard"
  account_replication_type = "GRS"

  tags = {
    Service    = "Early career framework for social work"
    Product    = "Early career framework for social work"
    Portfolio  = "Children and Families"
    Environment = "Dev"
  }
}





