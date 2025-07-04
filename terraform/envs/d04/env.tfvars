# Enforced by central policy, so matching the correct tag means eliminating false change plans
# This is most likely fixed at the subscription level, rather than a virtual environment within
# subscription.
environment_tag        = "Dev"
azure_region           = "westeurope"
environment            = "development"
resource_name_prefix   = "${project_code}d04"
primary_resource_group = "${project_code}d04-swip-rg"
environment_audience   = "user"
acr_name               = "${project_code}d01acr"
# The container registry is shared from the d01 instance
acr_resource_group          = "${project_code}d01-swip-rg"
acr_sku                     = "Basic"
admin_enabled               = false
asp_sku_moodle              = "B2"
asp_sku_maintenance         = "B2"
asp_sku_services            = "B1"
days_to_expire              = "365"
kv_purge_protection_enabled = false
moodle_instances = {
  # Capability to add multiple instances in the future (secondary = {} etc.)
  # They will each have their own dedicated DB
  primary = {}
}
# Enable dev friendly auth services features in dev environment
auth_service_app_settings = {
  "FEATUREFLAGS__ENABLEDEVELOPEREXCEPTIONPAGE" = "true"
  "FEATUREFLAGS__ENABLESWAGGER"                = "true"
  "DATABASESEED__ORGAGANISATIONID"             = "00000000-0000-0000-0000-000000000001"
  "DATABASESEED__ORGANISATIONNAME"             = "Test Organisation"
  "DATABASESEED__PERSONID"                     = "00000000-0000-0000-0001-000000000001"
  "DATABASESEED__ROLEID"                       = 800
  "DATABASESEED__ONELOGINSUBJECT"              = "urn:fdc:gov.uk:2022:KaCIMs1jlJNWz-TQ9Rq8McFXfBwy6JgYbsUNLIEpqKo"
  "DATABASESEED__ONELOGINEMAIL"                = "swip.test@education.gov.uk"
}
# Needs wiring up
one_login_client_id = "vZkWR2x4iDjtxvp22UolB4psq_Y"
moodle_app_settings = {
  "MOODLE_SWITCH_OFF_GOVUK_THEMING" = "true"
  "MOODLE_SWITCH_OFF_OAUTH"         = "true"
  "BASIC_AUTH_ENABLED"              = "true"
}

user_management_app_settings = {
  "BASIC_AUTH_ENABLED" = "true"
}
