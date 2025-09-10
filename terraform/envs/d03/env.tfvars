# Enforced by central policy, so matching the correct tag means eliminating false change plans
# This is most likely fixed at the subscription level, rather than a virtual environment within
# subscription.
environment_tag        = "Dev"
azure_region           = "westeurope"
environment            = "development"
resource_name_prefix   = "${project_code}d03"
primary_resource_group = "${project_code}d03-swip-rg"
environment_audience   = "user"
acr_name               = "${project_code}d01acr"
# The container registry is shared from the d01 instance
acr_resource_group                 = "${project_code}d01-swip-rg"
acr_sku                            = "Basic"
admin_enabled                      = false
asp_sku_moodle                     = "B2"
asp_sku_maintenance                = "B2"
asp_sku_services                   = "B1"
asp_sku_notification               = "B1"
asp_sku_db_jobs                    = "B1"
days_to_expire                     = "365"
kv_purge_protection_enabled        = false
moodle_max_data_storage_size_in_gb = 5
postgresql_sku                     = "B_Standard_B1ms"
frontdoor_sku                      = "Standard_AzureFrontDoor"
key_vault_sku                      = "standard"
log_analytics_sku                  = "PerGB2018"
moodle_instances = {
  # Capability to add multiple instances in the future (secondary = {} etc.)
  # They will each have their own dedicated DB
  primary = {}
}
# Enable dev friendly auth services features in dev environment
auth_service_app_settings = {
  "FEATUREFLAGS__ENABLEDEVELOPEREXCEPTIONPAGE" = "true"
  "FEATUREFLAGS__ENABLESWAGGER"                = "true"
  "DATABASESEED__PERSONID"                     = "00000000-0000-0000-0001-000000000001"
  "DATABASESEED__ROLEID"                       = 1000
  "DATABASESEED__ONELOGINEMAIL"                = "swip.test@education.gov.uk"
}
one_login_client_id = "kxYIW8mpUue-vHUFygaIBBHsNYY"
moodle_app_settings = {
  "MOODLE_SWITCH_OFF_GOVUK_THEMING" = "true" # Should be false for prod environment
  "MOODLE_SWITCH_OFF_OAUTH"         = "true" # Should be false for prod environment
  "MOODLE_PERSISTED_FILE_SYNC"      = "true"
}

user_management_app_settings = {
  "FEATUREFLAGS__ENABLEPLUSEMAILSTRIPPING" = "true"
}

db_backup_blob_sa_name = "sabackups"

magic_links_enabled = true