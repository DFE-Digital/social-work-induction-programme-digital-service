# Github and Terraform Secrets

This section gives an overview of the Github and Terraform secrets used within the DevOps processes.

See the [Terraform / Github Actions guidelines page](https://github.com/DFE-Digital/social-work-induction-programme-digital-service/blob/main/docs/devops/Terraform%20and%20GA%20Guidelines.md) for advice on when to use Github Actions or Terraform to store secrets.

## Github Secrets

These can be configured in the [Github Secrets and variables config section](https://github.com/DFE-Digital/social-work-induction-programme-digital-service/settings/secrets/actions) and can either be repository scoped (global) or environment-specific.

## Repository Scoped Secrets

| Secret Name | Usage | 
| -------- | ------- | 
| AZURE_KUDU_SSH_PASSWORD | Global password for the Azure Kudu (debugging) SSH user. This is required to configure Kudu for images and subsequently establish remote SSH sessions with app services in Github Actions |
| BASIC_AUTH_PASSWORD_TEAM_ENVIRONMENTS | Global basic auth password for project team environments |
| BASIC_AUTH_PASSWORD_USER_ENVIRONMENTS | Global basic auth password for environments oriented towards end users, e.g. for research and feedback |

## Environment Scoped Secrets

| Secret Name | Usage | 
| -------- | ------- | 
| AZ_CLIENT_ID | Client ID to use when authenticating with Azure |
| AZ_CLIENT_SECRET | Secret to be used in conjunction with the Client ID when authenticating with Azure. Mostly unused as we use `az login` with just Client ID and OIDC |
| AZ_SUBSCRIPTION_ID | Subscription ID to be used when authenticating with Azure |
| AZ_TENANT_ID | Tenant ID to be used when authenticating with Azure |
| MOODLE_ADMIN_PASSWORD | Password assigned to the admin user for initial database configuration in Moodle |

## Terraform / Key Vault Secrets

We use Terraform to declare secrets and subsequently store them in the environment specific key vault.

| Secret Name | Usage | 
| -------- | ------- | 
| AuthService-ClientSecret | Secret to be used by clients to authenticate with the auth service |
| BlobStorage-ConnectionString | Connection string required to authenticate with Azure blob storage. |
| Database-AdminPassword | Automatically created admin password for Moodle postgres database server |
| FileStorage-AccessKey | Access key required to connect to the Azure Files share |
| FileStorage-ConnectionString | Connection string required to connect to the Azure Files share |
| Moodle-WebServicePassword | Password for the Moodle web service user |
| Moodle-WebServiceToken | Authentication token generated for the web service user, from the user's name and password |
| Sites-BasicAuthPassword | Basic auth password for the system websites - Moodle and User Management. This will either be the project team password `BASIC_AUTH_PASSWORD_TEAM_ENVIRONMENTS` or `BASIC_AUTH_PASSWORD_USER_ENVIRONMENTS` dependent on whether the environment is for use by the project team or end users |

## Terraform / Key Vault Certificates

Certificates are also created and stored by Terraform in the key vault.

| Certificate Name | Usage | 
| -------- | ------- | 
| AuthService-OneLoginKeyPair | Onelogin private / public key pair required to sign the GovUK Onelogin service requests |
| AuthService-OpenIddictEncryptionCert | Auth Service OIDC encryption certificate |
| AuthService-OpenIddictSigningCert | Auth Service OIDC signing certificate |
