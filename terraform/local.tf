locals {
  # Common tags to be assigned to resources
  common_tags = {
    "Environment"      = var.environment_tag
    "Parent Business"  = var.parent_business_tag
    "Product"          = var.product_tag
    "Service Offering" = var.service_offering_tag
  }
  basic_auth_password = var.environment_audience == "team" ? var.basic_auth_password_team_environments : var.basic_auth_password_user_environments
}
