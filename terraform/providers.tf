# Configure the Azure provider
terraform {
  required_providers {
    # The following versions are pinned for build repeatability and should be reviewed and
    # updated regularly
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "= 4.43.0"
    }
    random = {
      source  = "hashicorp/random"
      version = "= 3.7.2"
    }
    external = {
      source  = "hashicorp/external"
      version = "= 2.3.5"
    }
  }

  # Terraform is version pinned within the Github Actions workflow file
  # required_version = ???

  backend "azurerm" {
    use_oidc = true
  }
}

provider "azurerm" {
  features {
    resource_group {
      prevent_deletion_if_contains_resources = false
    }

    key_vault {
      purge_soft_delete_on_destroy    = true
      recover_soft_deleted_key_vaults = true
    }
  }
}

data "azurerm_client_config" "az_config" {}
