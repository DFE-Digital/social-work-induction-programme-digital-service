# Enforced by central policy, so matching the correct tag means eliminating false change plans
# This is most likely fixed at the subscription level, rather than a virtual environment within
# subscription.
environment_tag                   = "Dev"
azure_region                      = "westeurope"
environment                       = "development"
resource_name_prefix              = "${project_code}d01"
primary_resource_group            = "${project_code}d01-swip-rg"
environment_audience              = "team"
create_and_own_container_registry = true
acr_name                          = "${project_code}d01acr"
acr_sku                           = "Basic"
admin_enabled                     = false
asp_sku_moodle                    = "B2"
asp_sku_maintenance               = "B2"
asp_sku_services                  = "B1"
asp_sku_notification              = "B1"
days_to_expire                    = "365"
kv_purge_protection_enabled       = false
moodle_instances = {
  # Capability to add multiple instances in the future (secondary = {} etc.)
  # They will each have their own dedicated DB
  primary = {}
}
# Enable dev friendly auth services features in dev environment
auth_service_app_settings = {
  "FEATUREFLAGS__ENABLEDEVELOPEREXCEPTIONPAGE" = "true"
  "FEATUREFLAGS__ENABLESWAGGER"                = "true"
  "DATABASESEED__ORGANISATIONID"             = "00000000-0000-0000-0000-000000000001"
  "DATABASESEED__ORGANISATIONNAME"             = "Test Organisation"
  "DATABASESEED__PERSONID"                     = "00000000-0000-0000-0001-000000000001"
  "DATABASESEED__ROLEID"                       = 800
  "DATABASESEED__ONELOGINEMAIL"                = "swip.test@education.gov.uk"
}
one_login_client_id = "p4yA1KMFQIoQbqmtntQZPTfdN_I"
moodle_app_settings = {
  "MOODLE_SWITCH_OFF_GOVUK_THEMING" = "false"
  "MOODLE_SWITCH_OFF_OAUTH"         = "false"
  "BASIC_AUTH_ENABLED"              = "true"
  "MOODLE_PERSISTED_FILE_SYNC"      = "true"
}
user_management_app_settings = {
  "BASIC_AUTH_ENABLED" = "true"
}
moodle_max_data_storage_size_in_gb = 5
storage_redundancy = "LRS"
blob_storage_account_tier = "Standard"
# Standard pay as you go HDD file share 
file_storage_account_tier = "Standard"
