# Enforced by central policy, so matching the correct tag means eliminating false change plans
# This is most likely fixed at the subscription level, rather than a virtual environment within
# subscription.
environment_tag = "Dev"
azure_region = "westeurope"
environment = "development"
resource_name_prefix = "${project_code}d01"
primary_resource_group = "${project_code}d01-swip-rg"
acr_name = "${project_code}d01acr"
acr_sku = "Basic"
admin_enabled = false
asp_sku = "B1"
days_to_expire = "365"
kv_purge_protection_enabled = false
moodle_instances = {
  # Capability to add multiple instances in the future (secondary = {} etc.)
  # They will each have their own dedicated DB
  primary = {}
}
