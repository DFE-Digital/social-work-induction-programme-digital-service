# Terraform / Github Actions Guidelines

This section outlines guidelines to be used when developing the Terraform code / Github Actions workflows.

# Naming Conventions and Tagging

1. When naming Terraform resources, use the general pattern: `<project-code><environment-name><resource-abbreviation><component-name>`, e.g. for the auth service: `${var.resource_name_prefix}-wa-auth-service` which translates to: `s205d01-wa-auth-service`, where `d01` is the environment and `wa` is an abbreviation for web app.

2. Most Azure resources support tagging, and the `common_tags` terraform variable declared in `/terraform/local.tf` can be used for this purpose. It's also good practice to ignore tagging changes in the event that central policy changes or manipulates them. E.g. 

```
resource "azurerm_container_registry" "acr" {
  ...
  tags = local.common_tags

  lifecycle {
    ignore_changes = [tags]
  }
}
```

## Variables / Secrets

1. If a variable is global, it should be stored at the repository level, otherwise the environment level. Variables should be stored in Github Actions if:
   - They are non-sensitive, such as user names, email addresses or refer to software versions. 
   - They are only required by Github Actions and not the Terraform code.

2. For Terraform, global variables should be declared in `/terraform.envs/global.tfvars` and environment specific variables in `/terraform/envs/eXX/env.tfvars`. Variables should be declared in Terraform if:
   - They are accessed by only the Terraform code or both Terraform and Gitub Actions.

3. If you need to access a Terraform variable in Github Actions, use the `pre-process-terraform-variables` custom action.

4. To inject a new Github Actions secret or variable into the `terraform` workflow, add it to the `Terraform Plan` step and also make sure there is a corresponding declaration in `/terraform/variables.tf`. Without the declaration, Terraform will produce a warning.

5. When declaring variables in Terraform, remember to flag them as sensitive if they relate to credentials, so that Terraform doesn't echo them to the logs. This is especially true if, for instance, secrets are maintained outside of Github Actions but are still accessed by workflows.

6. In general, prefer key vaults for storing secrets. With key vaults, team members are still able to view secret contents if necessary, unlike Github Actions. Github Actions secrets should be used for more 'static' secrets, such as the basic auth password for user facing sites, or the Kudu SSH password for containers.

7. If accessing a key vault secret in a Github Actions workflow, remember to use the `echo "::add-mask::$VAL"` statement to mask the secret value in log output.

## Structure

1. The Terraform implements the concept of a modular stack of resources which exists to support the operations of the system components. When introducing a new resource, consider whether it will be shared by all of the services (e.g. a key vault, database server etc) or the service is component specific. Add it to the stack if it can be accessed by all components, or declare it with the component if it is specific.
   
2. Create a Terraform module where there is lots of repeated boilerplate code for the resource in question. Currently this has been done for certificates, web apps and the environment stack.

3. Use `/terraform` to house the components for a particular stack. E.g. introducing a new search service should results in a `/terraform/search-service.tf` declaring the resources which directly comprise the resource and those resources which are service specific. Inspect `/terraform/auth-service.tf` for illustration. It declares a web app module, but also has a number of resources such as database, certificates and secrets which are specific to the resource.

## General

1. Pin Terraform providers to specific versions in `/terraform/providers.tf`. This will prevent unexpected issues when running Terraform after providers have been upgraded. Review this at least monthly to update to new providers. 

2. Leave the Terraform provisioning workflow clean - without warnings. These can distract when real issues come along.

3. The Terraform workflow is idempotent, in that if its state related resources aren't present, it will create the missing ones. This means that an entire environment can be bootstrapped simply by adding a new environment file to `/terraform/envs/eXX/env.tfvars` and updating the Terraform workflow with the additional environment name, e.g. `t01`. Make sure any modifications to the workflow do not affect this ability.
