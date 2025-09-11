# Component Overview / Deployment

## Components Overview

| Component | Description | Azure Application Service Plan | Workflows |
| -------- | ------- | --------- | ------- |
| Infrastructure | Uses Terraform to provide the Azure infrastructure / resources for a specific environment | - | terraform.yaml / terraform-destroy.yaml |
| Proxy service | Used as an SSH proxy to connect through to the database and perform migrations / updates | Maintenance service plan | build-and-optionally-deploy-proxy-service.yml / deploy-proxy-service.yml |
| Auth service | Uses Onelogin to authenticate users in User Management | Services service plan | build-and-optionally-deploy-auth-service.yml / deploy-auth-service.yml |
| User management | Provides user admin features for the Moodle user base | Services service plan | build-and-optionally-deploy-user-management.yml / deploy-user-management.yml |
| Moodle web site | The main Moodle website itself | Moodle service plan | build-and-optionally-deploy-moodle-apps.yml / deploy-moodle-app.yml |
| Moodle cron service | Stand-alone service (copy of the Moodle web site) used to execute the periodic Moodle cron maintenance job | Maintenance service plan | Same as Moodle - deploys both services in the same workflow as they use the same image |

## Deployment Patterns

Generally, each system component has a build and optionally deploy to dev workflow, as well as a dedicated deployment only workflow. The deployment only workflow expects an image label in the example formats below.

## Example Image Labels

Note that the Moodle image contains the branch version in the label for ease of reference. All images contain the build date and commit hash and are targeted at the `Dev` environment until a development workflow (Git Flow, Github Flow etc) has been established. 

| Component | Repository | Example Label |
| -------- | ------- | --------- |
| Proxy service | dfe-digital-swip-digital-service/proxy-service | 20250625.23b87c3.dev |
| Auth service | dfe-digital-swip-digital-service/auth-service | 20250704.316f79d.dev | 
| User management | dfe-digital-swip-digital-service/user-management | 20250625.23b87c3.dev |
| Moodle web site | dfe-digital-swip-digital-service/wa-moodle | 405-20250703.20c67ca.dev |
| Moodle cron service | dfe-digital-swip-digital-service/wa-moodle | 405-20250703.20c67ca.dev |

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

### Moodle Website

- Begins with deployment of app service
- App service container startup:
  - Start the SSH service for Kudu assisted troubleshooting / deployment
  - Environment variable `IS_CRON_JOB_ONLY` is set `false` indicating the container will serve the main Moodle website (rather than executing the periodic Moodle cron job)
  - Saves a copy of the non-sensitive environment variables locally, so these can be reused by the Moodle SSH deployment process
  - If the attached Azure Files share is populated with files, copies them over to the local `/var/wwww/moodledata` directory to ensure config / file persistence
  - If the attached Azure Files share is empty, it syncs from the local `/var/wwww/moodledata_ref` folder to the file share. This ensures that the share is initialised correctly in the first instance.
  - If environment variable `MOODLE_PERSISTED_FILE_SYNC` is set `true`, start a periodic background sync of files from `/var/wwww/moodledata` to the Azure Files share.
  - Switch OAuth off / on according to: `MOODLE_SWITCH_OFF_OAUTH` (makes development easier if switched off) 
  - Start the Apache web server
- Workflow waits for Apache to serve correct version string
- Establish a background SSH connection to the app service
- Instantiate SSH session environment variables from the saved startup file
- Add additional secrets required by Moodle maintenance process and specified as part of the deploy
- Run `maintain-moodle.sh` script:
  - If no install, run Moodle database creation / config
  - Run Moodle web service setup
  - OIDC plugin setup
  - Run Moodle upgrade process
  - Maintain custom migration table with success / failure result

### Moodle Cron Service

- Begins with deployment of app service
- App service container startup:
  - Start the SSH service for Kudu assisted troubleshooting / deployment
  - Environment variable `IS_CRON_JOB_ONLY` is set `true` indicating the container will take care of running the periodic Moodle cron job
  - Start periodic Moodle cron job
  - Switch OAuth off / on according to: `MOODLE_SWITCH_OFF_OAUTH` (makes development easier if switched off) 
  - Start the Apache web server
- Workflow waits for Apache to serve correct version string
