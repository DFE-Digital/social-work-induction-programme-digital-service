locals {
  # Common tags to be assigned to resources
  common_tags = {
    "Environment"      = var.environment_tag
    "Parent Business"  = var.parent_business_tag
    "Product"          = var.product_tag
    "Service Offering" = var.service_offering_tag
  }

  front_door_endpoint_ids = concat(
    [for instance in module.web_app_moodle : instance.front_door_endpoint_id],
    [module.auth_service.front_door_endpoint_id],
    [module.user_management.front_door_endpoint_id],
  )
}
