locals {
  # Common tags to be assigned to resources
  common_tags = {
    "Environment"      = var.environment
    "Parent Business"  = "Children and Families"
    "Product"          = "Social Work Induction Programme"
    "Service Offering" = "Social Work Induction Programme"
  }
}
