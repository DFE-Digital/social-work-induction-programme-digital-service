# Github and Terraform Variables

This section gives an overview of the Github and Terraform variables used within the DevOps processes.

See the [Terraform / Github Actions guidelines page](https://github.com/DFE-Digital/social-work-induction-programme-digital-service/blob/main/docs/devops/Terraform%20and%20GA%20Guidelines.md) for advice on when to use Github Actions or Terraform to store variables.

## Github Variables

These can be configured in the [Github Secrets and variables config section](https://github.com/DFE-Digital/social-work-induction-programme-digital-service/settings/secrets/actions) and can either be repository scoped (global) or environment-specific.

## Repository Scoped Variables

| Variable Name | Usage | 
| -------- | ------- | 
| AZURE_KUDU_SSH_USER | Global name of user required by Kudu to SSH to app service containers |
| BASIC_AUTH_USER | Global name of user name required for basic auth on end user facing sites (e.g. for user research) |
| MOODLE_OIDC_PLUGIN_RELEASE_URL | Global url of OIDC plugin to use when building Moodle images |

## Environment Scoped Variables

| Variable Name | Usage | 
| -------- | ------- | 
| AUTH_SERVICE_CLIENT_ID | Client ID to be used by Github Actions when authenticating to Azure |
| EMAIL_SUPPORT_ADDRESS | Email address used by Azure action group to send alert emails to |
| MOODLE_ADMIN_EMAIL | Email address of admin user to be used when configuring Moodle |
| MOODLE_ADMIN_USER | User name of admin user to be used when configuring Moodle |
| MOODLE_WEB_SERVICE_NAME | Name of Moodle web service to be used when configuring the web service |
| MOODLE_WEB_SERVICE_USER | Name of Moodle web service user to be used when configuring the web service |
| MOODLE_WEB_SERVICE_USER_EMAIL | Email address of Moodle web service user to be used when configuring the web service |

## Terraform Variables

These can either be declared globally in https://github.com/DFE-Digital/social-work-induction-programme-digital-service/blob/main/terraform/envs/global.tfvars or per environment as for example: https://github.com/DFE-Digital/social-work-induction-programme-digital-service/blob/main/terraform/envs/d01/env.tfvars

### Globally Scoped Variables

| Variable Name | Usage | 
| -------- | ------- | 
| project_code | SWIP programme project code |
| moodle_site_fullname | Long site name to be used when configuring Moodle |
| moodle_site_shortname | Short abbreviation of site name to be used when configuring Moodle |
| webapp_storage_account_name | Generic name of blob storage account for apps - will be preceded by environment prefix in a particular environment |
| parent_business_tag | The value of the 'Parent Business' tag for resources in Azure |
| product_tag | The value of the 'Product' tag for resources in Azure |
| service_offering_tag | The value of the 'Service Offering' tag for resources in Azure |
| assign_delivery_team_key_vault_permissions | Whether or not to assign the delivery team group in Azure permissions to view vault secrets and contents in Azure. (This is purely for convenience - the team is capable of giving themselves these permissions.) |

## Environment Scoped Variables

| Variable Name | Usage | 
| -------- | ------- | 
| environment_tag | The value of the 'Environment' tag for resources in Azure |
| environment | Environment type - development, test, production. Not currently used. Was used previously to determine certain app settings based on development / test, but this isn't a good pattern. |
| resource_name_prefix | String to prefix resources with in Azure. Makes it easy to link resources to environments |
| primary_resource_group | Resource group to add environment resources to |
| environment_audience | team or user - these environments audiences have different basic auth passwords |
| acr_name | Name of Azure container registry to use for app service / function images. Environments in different subscriptions can share container registries - e.g. dev / test |
| acr_resource_group  | Resource group which Azure container registry resides in |
| acr_sku | Azure SKU for container registry |
| admin_enabled | Corresponds to the Terraform Azure container registry property admin_enabled. If true, then there is a single admin user defined for interactions with the registry. We currently set it to false in all environments. |
| asp_sku_moodle | The Azure SKU for the Moodle Application Service Plan |
| asp_sku_maintenance | The Azure SKU for the Moodle Maintenance Service Plan (cron job + proxy service) |
| asp_sku_services | The Azure SKU for the Services Application Service Plan (user management + auth service etc) |
| days_to_expire | Number of days to expire for password / secrets. Currently not used, but may be required as we manage more secrets through the key vault. |
| kv_purge_protection_enabled | Whether or not to enable purge protection for the key vault. When purge protection is on, a vault or an object in the deleted state cannot be purged until the retention period has passed. |
| moodle_instances | Originally, this was added to support multiple Moodle instances per environment. However, there are two many Moodle dependent services for this to be viable now, so each environment simply implements a single primary Moodle instance. |
| auth_service_app_settings | Environment specific app settings to merge with the auth service app settings. FEATUREFLAGS__ENABLEDEVELOPEREXCEPTIONPAGE, whether or not to enable the developer exception page in .NET. FEATUREFLAGS__ENABLESWAGGER, whether or not to enable swagger.  |
| one_login_client_id | The Onelogin client ID either obtained from the admin integration site environment config, or the production environment config received from the central Onelogin team. Note that when creating a new environment, this can be left empty until the Onelogin config has been created / received. |
| moodle_app_settings | Environment specific app settings to merge with the Moodle app settings. MOODLE_SWITCH_OFF_GOVUK_THEMING, whether or not to switch off the GovUK theming, to make admin easier in the dev and test environments. MOODLE_SWITCH_OFF_OAUTH, whether or not to switch of OAUTH, again to make admin easier. BASIC_AUTH_ENABLED, whether or not basic auth is enabled for the environment. (It should be for all non-test environments.) |
| user_management_app_settings | Environment specific app settings to merge with the user management app settings. BASIC_AUTH_ENABLED, whether or not basic auth is enabled for the environment. (It should be for all non-test environments.) |
