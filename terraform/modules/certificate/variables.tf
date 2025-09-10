variable "key_vault_id" {
  type        = string
  description = "Key vault in which to create certificate"
}

variable "cert_name" {
  description = "Name of certificate to be created"
  type        = string
}

variable "common_name" {
  description = "Common name (CN) of certificate"
  type        = string
  default     = "swip.education.gov.uk"
}

variable "key_size" {
  type        = number
  description = "The key size for the certificate"
  default     = 2048
}

variable "validity_in_months" {
  type        = number
  description = "The number of months the certificate is valid for"
  default     = 6
}

variable "days_before_expiry_to_renew" {
  type        = number
  description = "The number of days before the certificate expires to renew"
  default     = 30
}