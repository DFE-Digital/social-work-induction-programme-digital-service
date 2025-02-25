variable "resource_name_prefix" {
  description = "Prefix for resource names"
  type        = string
}

variable "tags" {
  description = "Resource tags"
  type        = map(string)
}

variable "resource_group" {
  description = "Name of the Azure Resource Group to deploy resources"
  type        = string
}
