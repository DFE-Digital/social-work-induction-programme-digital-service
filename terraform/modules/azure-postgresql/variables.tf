variable "environment" {
  description = "Environment to deploy resources"
  type        = string
}

variable "location" {
  description = "Name of the Azure region to deploy resources"
  type        = string
}

variable "resource_group" {
  description = "Name of the Azure Resource Group to deploy resources"
  type        = string
}

variable "resource_name_prefix" {
  description = "Prefix for resource names"
  type        = string
}

variable "vnet_name" {
  description = "Name of the virtual network"
  type        = string
}

variable "vnet_id" {
  description = "ID of the virtual network"
  type        = string
}
