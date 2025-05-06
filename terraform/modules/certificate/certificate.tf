resource "azurerm_key_vault_certificate" "kv_cert" {
  name         = var.cert_name
  key_vault_id = var.key_vault_id

  certificate_policy {
    # Use Key Vault’s built-in self-signed issuer
    issuer_parameters {
      name = "Self"
    }

    # Configure the key properties
    key_properties {
      exportable = true
      key_size   = var.key_size
      key_type   = "RSA"
      reuse_key  = true
    }

    # Make it a PFX
    secret_properties {
      content_type = "application/x-pkcs12"
    }

    x509_certificate_properties {
      subject            = "CN=${var.common_name}"
      validity_in_months = var.validity_in_months
      extended_key_usage = [
        "1.3.6.1.5.5.7.3.1"   # TLS server auth
      ]
      key_usage = [
        "cRLSign",
        "dataEncipherment",
        "digitalSignature",
        "keyAgreement",
        "keyCertSign",
        "keyEncipherment",
      ]      
    }

    lifetime_action {
      action {
        action_type = "AutoRenew"
      }
      trigger {
        days_before_expiry = var.days_before_expiry_to_renew
      }
    }
  }
}