resource "azurerm_cdn_frontdoor_endpoint" "front_door_endpoint_web" {
  cdn_frontdoor_profile_id = var.front_door_profile_web_id
  name                     = "${var.resource_name_prefix}-fd-endpoint-web-${var.web_app_short_name}"
  tags                     = var.tags

  lifecycle {
    ignore_changes = [tags]
  }
}

resource "azurerm_cdn_frontdoor_origin_group" "front_door_origin_group_web" {
  cdn_frontdoor_profile_id = var.front_door_profile_web_id
  name                     = "${var.resource_name_prefix}-fd-origin-group-web-${var.web_app_short_name}"

  health_probe {
    interval_in_seconds = 60
    protocol            = "Https"
    request_type        = "GET"
    path                = var.health_check_path
  }

  load_balancing {
    sample_size                 = 4
    successful_samples_required = 2
  }
}

# Shared resources are now managed in environment-stack module
# This module only handles endpoint-specific configuration

# Conditionally create security policy only if Magic Links are enabled
resource "azurerm_cdn_frontdoor_security_policy" "magic_link_security" {
  count                    = var.magic_link_waf_policy_id != null ? 1 : 0
  name                     = "${var.resource_name_prefix}-fd-security-magic-link-${var.web_app_short_name}"
  cdn_frontdoor_profile_id = var.front_door_profile_web_id

  security_policies {
    firewall {
      cdn_frontdoor_firewall_policy_id = var.magic_link_waf_policy_id

      association {
        domain {
          cdn_frontdoor_domain_id = azurerm_cdn_frontdoor_endpoint.front_door_endpoint_web.id
        }
        patterns_to_match = ["/*"]
      }
    }
  }

  depends_on = [azurerm_cdn_frontdoor_endpoint.front_door_endpoint_web]
}


resource "azurerm_cdn_frontdoor_origin" "front_door_origin_web" {
  cdn_frontdoor_origin_group_id  = azurerm_cdn_frontdoor_origin_group.front_door_origin_group_web.id
  certificate_name_check_enabled = false
  host_name                      = azurerm_linux_web_app.webapp.default_hostname
  http_port                      = 80
  https_port                     = 443
  origin_host_header             = azurerm_linux_web_app.webapp.default_hostname
  priority                       = 1
  weight                         = 1
  name                           = "${var.resource_name_prefix}-fd-origin-web-${var.web_app_short_name}"
  enabled                        = true

  depends_on = [azurerm_cdn_frontdoor_origin_group.front_door_origin_group_web]
}

resource "azurerm_cdn_frontdoor_route" "front_door_route_web" {
  name                          = "${var.resource_name_prefix}-fd-route-web-${var.web_app_short_name}"
  cdn_frontdoor_endpoint_id     = azurerm_cdn_frontdoor_endpoint.front_door_endpoint_web.id
  cdn_frontdoor_origin_group_id = azurerm_cdn_frontdoor_origin_group.front_door_origin_group_web.id
  cdn_frontdoor_origin_ids      = [azurerm_cdn_frontdoor_origin.front_door_origin_web.id]

  # Conditionally use shared rule set only if Magic Links are enabled
  cdn_frontdoor_rule_set_ids = var.magic_link_rule_set_id != null ? [var.magic_link_rule_set_id] : []

  forwarding_protocol    = "MatchRequest"
  https_redirect_enabled = true
  patterns_to_match      = ["/*"]
  supported_protocols    = ["Http", "Https"]

  depends_on = [
    azurerm_cdn_frontdoor_endpoint.front_door_endpoint_web,
    azurerm_cdn_frontdoor_origin_group.front_door_origin_group_web,
    azurerm_cdn_frontdoor_origin.front_door_origin_web
  ]
}
