variable "resource_name_prefix" {
  description = "Prefix for resource names"
  type        = string
}

variable "magic_links_enabled" {
  description = "Whether magic links are enabled"
  type        = bool
  default     = false
}

variable "moodle_instances" {
  description = "Map of Moodle instances"
  type        = map(any)
  default     = {}
}

variable "frontdoor_sku" {
  description = "Front Door SKU"
  type        = string
}

variable "tags" {
  description = "Tags to apply to resources"
  type        = map(string)
  default     = {}
}

variable "key_vault_id" {
  description = "Key Vault ID"
  type        = string
}

variable "frontdoor_profile_id" {
  description = "Front Door Profile ID"
  type        = string
}

variable "frontdoor_profile_name" {
  description = "Front Door Profile Name"
  type        = string
}

variable "resource_group_name" {
  description = "Resource Group Name"
  type        = string
}
