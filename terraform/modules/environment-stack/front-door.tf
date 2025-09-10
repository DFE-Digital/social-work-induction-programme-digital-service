resource "azurerm_cdn_frontdoor_profile" "front_door_profile_web" {
  name                = "${var.resource_name_prefix}-fd-profile-web"
  resource_group_name = azurerm_resource_group.rg_primary.name
  sku_name            = var.frontdoor_sku
  tags                = var.tags

  lifecycle {
    ignore_changes = [tags]
  }
}

# Magic Link Resources - Only created when enabled
resource "random_password" "magic_link_token" {
  count   = var.magic_links_enabled ? 1 : 0
  length  = 32
  special = false # URL-safe, no special characters
  upper   = true
  lower   = true
  numeric = true
}

resource "azurerm_key_vault_secret" "magic_link_token" {
  count           = var.magic_links_enabled ? 1 : 0
  name            = "MagicLink-Token"
  value           = random_password.magic_link_token[0].result
  key_vault_id    = azurerm_key_vault.kv.id
  content_type    = "text/plain"
  expiration_date = timeadd(timestamp(), "24h")

  lifecycle {
    ignore_changes = [value] # Value will be updated by other workflows
  }

  depends_on = [azurerm_key_vault_access_policy.kv_gh_ap]
}

# Magic Link Authentication Rules Engine - Shared across all web apps
resource "azurerm_cdn_frontdoor_rule_set" "magic_link_rules" {
  count                    = var.magic_links_enabled ? 1 : 0
  name                     = "${var.resource_name_prefix}fdrulesetmagiclink"
  cdn_frontdoor_profile_id = azurerm_cdn_frontdoor_profile.front_door_profile_web.id
}

resource "azurerm_cdn_frontdoor_rule" "token_validation" {
  count                     = var.magic_links_enabled ? 1 : 0
  name                      = "${var.resource_name_prefix}fdruletokenvalidation"
  cdn_frontdoor_rule_set_id = azurerm_cdn_frontdoor_rule_set.magic_link_rules[0].id
  order                     = 1
  behavior_on_match         = "Continue"

  conditions {
    query_string_condition {
      operator         = "Equal"
      match_values     = ["token=${azurerm_key_vault_secret.magic_link_token[0].value}"]
      negate_condition = false
    }
  }

  actions {
    response_header_action {
      header_action = "Overwrite"
      header_name   = "Set-Cookie"
      value         = "dev_auth=${azurerm_key_vault_secret.magic_link_token[0].value}; Secure; HttpOnly; SameSite=Strict"
    }

    url_redirect_action {
      redirect_type        = "Found"
      destination_hostname = ""
      destination_path     = ""
      query_string         = ""
      redirect_protocol    = "Https"
    }
  }

  depends_on = [azurerm_cdn_frontdoor_rule_set.magic_link_rules]
}

resource "azurerm_cdn_frontdoor_rule" "cookie_validation" {
  count                     = var.magic_links_enabled ? 1 : 0
  name                      = "${var.resource_name_prefix}fdrulecookievalidation"
  cdn_frontdoor_rule_set_id = azurerm_cdn_frontdoor_rule_set.magic_link_rules[0].id
  order                     = 2
  behavior_on_match         = "Continue"

  conditions {
    cookies_condition {
      cookie_name      = "dev_auth"
      operator         = "Equal"
      match_values     = [azurerm_key_vault_secret.magic_link_token[0].value]
      negate_condition = false
    }
  }

  actions {
    # Add a minimal action to prevent provider crash
    # This rule is used for conditional matching only
    request_header_action {
      header_action = "Overwrite"
      header_name   = "X-Magic-Link-Validated"
      value         = "true"
    }
  }

  depends_on = [azurerm_cdn_frontdoor_rule_set.magic_link_rules]
}

# Magic Link WAF Policy - Shared across all web apps
resource "azurerm_cdn_frontdoor_firewall_policy" "magic_link_waf" {
  count               = var.magic_links_enabled ? 1 : 0
  name                = "${var.resource_name_prefix}fdmagiclinkwafpolicy"
  resource_group_name = azurerm_resource_group.rg_primary.name
  sku_name            = var.frontdoor_sku
  mode                = "Prevention"

  custom_rule {
    name     = "AllowValidToken"
    priority = 1
    type     = "MatchRule"
    action   = "Allow"

    match_condition {
      match_variable = "QueryString"
      operator       = "Contains"
      match_values   = ["token=${azurerm_key_vault_secret.magic_link_token[0].value}"]
      # Note: selector cannot be used with QueryString match_variable
    }
  }

  custom_rule {
    name     = "AllowValidCookie"
    priority = 2
    type     = "MatchRule"
    action   = "Allow"

    match_condition {
      match_variable = "Cookies"
      selector       = "dev_auth"
      operator       = "Equal"
      match_values   = [azurerm_key_vault_secret.magic_link_token[0].value]
    }
  }

  # TODO: Re-enable once it's confirmed the above rules are working
  # custom_rule {
  #   name     = "BlockUnauthorized"
  #   priority = 100
  #   type     = "MatchRule"
  #   action   = "Block"

  #   match_condition {
  #     match_variable = "RemoteAddr"
  #     operator       = "IPMatch"
  #     match_values   = ["0.0.0.0/0"]
  #   }
  # }
}

# Data sources to reference Front Door endpoints created by web-app modules
# These avoid circular dependencies by referencing endpoints by name
data "azurerm_cdn_frontdoor_endpoint" "moodle_endpoints" {
  count               = var.magic_links_enabled ? length(var.moodle_instances) : 0
  name                = "${var.resource_name_prefix}-fd-endpoint-web-wa-moodle-${element(keys(var.moodle_instances), count.index)}"
  profile_name        = azurerm_cdn_frontdoor_profile.front_door_profile_web.name
  resource_group_name = azurerm_resource_group.rg_primary.name

  depends_on = [azurerm_cdn_frontdoor_profile.front_door_profile_web]
}

data "azurerm_cdn_frontdoor_endpoint" "auth_service_endpoint" {
  count               = var.magic_links_enabled ? 1 : 0
  name                = "${var.resource_name_prefix}-fd-endpoint-web-wa-auth-service"
  profile_name        = azurerm_cdn_frontdoor_profile.front_door_profile_web.name
  resource_group_name = azurerm_resource_group.rg_primary.name

  depends_on = [azurerm_cdn_frontdoor_profile.front_door_profile_web]
}

data "azurerm_cdn_frontdoor_endpoint" "user_management_endpoint" {
  count               = var.magic_links_enabled ? 1 : 0
  name                = "${var.resource_name_prefix}-fd-endpoint-web-wa-user-management"
  profile_name        = azurerm_cdn_frontdoor_profile.front_door_profile_web.name
  resource_group_name = azurerm_resource_group.rg_primary.name

  depends_on = [azurerm_cdn_frontdoor_profile.front_door_profile_web]
}

# Shared Security Policy - Uses data sources to avoid circular dependencies
resource "azurerm_cdn_frontdoor_security_policy" "magic_link_security" {
  count = var.magic_links_enabled ? 1 : 0
  name  = "${var.resource_name_prefix}-fd-security-magic-link"

  cdn_frontdoor_profile_id = azurerm_cdn_frontdoor_profile.front_door_profile_web.id

  security_policies {
    firewall {
      cdn_frontdoor_firewall_policy_id = azurerm_cdn_frontdoor_firewall_policy.magic_link_waf[0].id
      association {
        dynamic "domain" {
          for_each = concat(
            [for endpoint in data.azurerm_cdn_frontdoor_endpoint.moodle_endpoints : endpoint.id],
            [for endpoint in data.azurerm_cdn_frontdoor_endpoint.auth_service_endpoint : endpoint.id],
            [for endpoint in data.azurerm_cdn_frontdoor_endpoint.user_management_endpoint : endpoint.id],
          )
          content {
            cdn_frontdoor_domain_id = domain.value
          }
        }
        patterns_to_match = ["/*"]
      }
    }
  }

  depends_on = [
    azurerm_cdn_frontdoor_firewall_policy.magic_link_waf,
    data.azurerm_cdn_frontdoor_endpoint.moodle_endpoints,
    data.azurerm_cdn_frontdoor_endpoint.auth_service_endpoint,
    data.azurerm_cdn_frontdoor_endpoint.user_management_endpoint,
  ]
}
