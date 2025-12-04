# Component Overview / Deployment

## Components Overview

| Component | Description | Azure Application Service Plan | Workflows |
| -------- | ------- | --------- | ------- |
| Infrastructure | Uses Terraform to provide the Azure infrastructure / resources for a specific environment | - | terraform.yaml / terraform-destroy.yaml |
| Proxy service | Used as an SSH proxy to connect through to the database and perform migrations / updates | Maintenance service plan | build-and-optionally-deploy-proxy-service.yml / deploy-proxy-service.yml |
| Auth service | Uses Onelogin to authenticate users in User Management | Services service plan | build-and-optionally-deploy-auth-service.yml / deploy-auth-service.yml |
| User management | Provides user admin features for the user base | Services service plan | build-and-optionally-deploy-user-management.yml / deploy-user-management.yml |

## Deployment Patterns

Generally, each system component has a build and optionally deploy to dev workflow, as well as a dedicated deployment only workflow. The deployment only workflow expects an image label in the example formats below.

## Example Image Labels

All images contain the build date and commit hash and are targeted at the `Dev` environment until a development workflow (Git Flow, Github Flow etc) has been established. 

| Component | Repository | Example Label |
| -------- | ------- | --------- |
| Proxy service | dfe-digital-swip-digital-service/proxy-service | 20250625.23b87c3.dev |
| Auth service | dfe-digital-swip-digital-service/auth-service | 20250704.316f79d.dev | 
| User management | dfe-digital-swip-digital-service/user-management | 20250625.23b87c3.dev |

## Components Deployment

### Proxy Service

Should be deployed first in a new environment!

- Straightforward deployment of app service
- Proxy service starts SSH and waits for connections
- Lighttpd used to serve version string
- Workflow waits for lighttpd to serve correct version string

### Auth Service

- Migrations deployed first to the database server by establishing an SSH tunnel through the proxy server to the database server
- On successful application of migrations, app service itself is deployed
- Workflow waits for auth service to serve correct version string

### User Management

- Straightforward deployment of app service
- Workflow waits for user management service to serve correct version string
