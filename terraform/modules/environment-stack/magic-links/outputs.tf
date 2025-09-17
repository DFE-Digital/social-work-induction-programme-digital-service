output "magic_link_token_value" {
  description = "Shared magic link token value"
  value       = azurerm_key_vault_secret.magic_link_token[0].value
  sensitive   = true
}

output "magic_link_waf_policy_id" {
  description = "ID of the shared magic link WAF policy"
  value       = azurerm_cdn_frontdoor_firewall_policy.magic_link_waf[0].id
}

output "magic_link_rule_set_id" {
  description = "ID of the shared magic link rule set"
  value       = azurerm_cdn_frontdoor_rule_set.magic_link_rules[0].id
}

output "magic_link_security_policy_id" {
  description = "ID of the shared magic link security policy"
  value       = azurerm_cdn_frontdoor_security_policy.magic_link_security[0].id
}
