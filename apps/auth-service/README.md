# SWPDP digital service: `auth-service`

## Installation

See [LOCALDEV one-time installation](/README.md#localdev-one-time-installation) and [Starting LOCALDEV](/README.md#starting-localdev)
for installation instructions for the SWPDP digital service.

## Overview

Auth service (`auth-service`) provides an API for authentication and data retrieval/persistence. It is an ASP.NET Core 8 web application.

## One Login integration

During [LOCALDEV one-time installation](/README.md#localdev-one-time-installation), when setting up and running `auth-service`:

- `just cli migrate-db`:
  - Adds 1 row to the `users` table.
  - There are no rows in the `one_login_users`, `persons` or `person_roles` tables.

- `cd ~/swpdp/apps/auth-service; just watch-authz`:
  - Adds 1 row to the `persons` table, with `email_address` set to the default email address.
  - Adds 1 row to the `person_roles` table.
  - Adds 3 rows to the `person_search_attributes` table.
  - There are no rows in the `one_login_users` table.

- First use of One Login Simulator using the default email address:
  - Adds 1 row to the `one_login_users` table, with `person_id` populated with `persons` > `person_id`.
  - Results in a successful `user-service` login.

The local `onelogin-simulator` is a Node application which acts as the OpenID authentication server in place of the real One Login service. It runs entirely locally as a container and allows us full control over what the response is from the auth flow. It also features a [config](https://github.com/govuk-one-login/simulator/blob/main/docs/configuration.md) endpoint that can be used to change the responses of the service on-the-fly. Further information and guides can be found in the official [GitHub repo](https://github.com/govuk-one-login/simulator) for the simulator.

The development configurations for the auth service are already pre-configured to use the simulator by default

Note: The `onelogin-simulator` does _not_ emulate the UI of the One Login service; it will not show the user any login pages or forms, it will simply return whatever response it is configured to give. By default, it will log the user in successfully with the default values specified [here](https://github.com/govuk-one-login/simulator).

### Switch `auth-service` from One Login Simulator to GOV.UK One Login integration environment:
In order to use the official [GOV.UK One Login integration environment](https://docs.sign-in.service.gov.uk/integrate-with-integration-environment),
the application needs to be configured with the API account details.

1. Edit `apps/auth-service/Dfe.Sww.Ecf/src/Dfe.Sww.Ecf.AuthorizeAccess/appsettings.Development.json`

2. Switch the OneLogin configuration from **Local simulator** to **Integration environment**:
```bash
--CHANGE---8<-----8<-----8<-----8<-----8<-----8<-----8<-----
  "OneLogin": {
    "ClientId": "<CLIENT ID>",
    "PrivateKeyPem": "<PRIVATE KEY PEM>",
    "Url": "https://localhost:9010/onelogin"
  },
--TO---8<-----8<-----8<-----8<-----8<-----8<-----8<-----
  "OneLogin": {
     "Url": "https://oidc.integration.account.gov.uk"
  }
-----8<-----8<-----8<-----8<-----8<-----8<-----8<-----
```

3. Stop `auth-service` running locally and configure `ClientId` and `PrivateKeyPem` with the **integration environment credentials**:
```bash
just set-secret OneLogin:ClientId "exampleClientId"
just set-secret OneLogin:PrivateKeyPem "-----BEGIN PRIVATE KEY-----\nExamplePrivateKeyPem\nWithNewLinesEscaped\nSoItsOnASingleLine\n-----END PRIVATE KEY-----"
```

4. Restart `auth-service` locally:
```bash
cd ~/swpdp/apps/auth-service
just watch-authz
```

5. Go to `user-management` (https://localhost:7244) and click **Sign in with GOV.UK One Login**.
Rather than being redirected to the local One Login Simulator, you will be redirected to
https://signin.integration.account.gov.uk/sign-in-or-create

6. Click **Sign in**, enter the default email address and complete the login process. If successful, you will be redirected to the `user-management` dashboard. NB: Using the default email address, the same row in `persons` is used for this login, although a new row is added to `one_login_users` with a different `subject` value.

### Reverting `auth-service` to use One Login Simulator

1. Stop `auth-service` running locally and configure `ClientId` and `PrivateKeyPem` with the **simulator cedentials**:
```bash
just set-secret OneLogin:ClientId "exampleClientId"
just set-secret OneLogin:PrivateKeyPem "-----BEGIN PRIVATE KEY-----\nExamplePrivateKeyPem\nWithNewLinesEscaped\nSoItsOnASingleLine\n-----END PRIVATE KEY-----"
```
2. Restart `auth-service` locally:
```bash
cd ~/swpdp/apps/auth-service
just watch-authz
```

### Creating an additional One Login user

1. Manually add a row to the `persons` and `person_roles` tables, assigning them the admin role:
```bash
INSERT INTO public.persons
  (person_id, created_on, first_name, last_name, email_address)
VALUES
  ('99999999-9999-9999-9999-999999999999', NOW(), 'New', 'User', 'newuser@education.gov.uk');

INSERT INTO public.person_roles
	(person_id, role_id)
VALUES
	('99999999-9999-9999-9999-999999999999', 1000);
```

2. Go to `user-management` (https://localhost:7244) and click **Sign in with GOV.UK One Login**.

3. Authenticate
   - If `auth-service` is configured to use the One Login Simulator:
     - Change the default **Subject (sub)** field to `999`
     - Change the default **Email** field to `newuser@education.gov.uk`
     - Click **Continue**
     - The One Login Simulator login page will be re-displayed.
   - If `auth-service` is configured to use the GOV.UK One Login integration environment:
     - Click **Sign in**
     - Enter email address/password and complete OTP authentication.
     - A **Request vtr is not permitted** error page will be shown: **this is expected**.

4. Manually link the One Login login with the `auth-service` login:
   - The `one_login_users` table will have a new row with `email` set to the recently-attempted login.
   - Update this `one_login_users` row, setting `person_id` to `99999999-9999-9999-9999-999999999999`

6. Repeat **steps 2 and 3** above: One Login should now allow this login and redirect the user to the `user-management` dashboard.

## Testing

To generate the test database and execute the migrations against it, run:

```bash
cd ~/swpdp/apps/auth-service
just cli migrate-db --connection TestDbConnection
```

## Feature Flags

The auth service uses feature flags to control various functionality. Feature flags are configured in the `appsettings.json` files and can be overridden using environment variables or other configuration sources.

### Available Feature Flags

| Flag                                    | Description                                                                                                                      | Default Value                                |
| --------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------- | -------------------------------------------- |
| `SupportEndToEndTesting`                | Enables end-to-end testing support and configuration                                                                             | `true` in Development, `false` in Production |
| `RequiresDbConnection`                  | Controls whether database connection is required for the service                                                                 | `false` in Development, `true` in Production |
| `EnableDeveloperExceptionPage`          | Enables detailed error pages in development for debugging                                                                        | `true` in Development, `false` in Production |
| `EnableMigrationsEndpoint`              | Enables database migration endpoints for development                                                                             | `true` in Development, `false` in Production |
| `EnableErrorExceptionHandler`           | Enables global error exception handler                                                                                           | `true` in Development, `false` in Production |
| `EnableContentSecurityPolicyWorkaround` | Enables Content Security Policy workaround for ASP.NET Core auto refresh (see https://github.com/dotnet/aspnetcore/issues/33068) | `true` in Development, `false` in Production |
| `EnableDfeAnalytics`                    | Enables DfE analytics tracking                                                                                                   | `false`                                      |
| `EnableSwagger`                         | Enables Swagger API documentation                                                                                                | `true` in Development, `false` in Production |
| `EnableSentry`                          | Enables Sentry error tracking and monitoring                                                                                     | `false`                                      |
| `EnableHttpStrictTransportSecurity`     | Enables HSTS security headers                                                                                                    | `false` in Development, `true` in Production |
| `EnableForwardedHeaders`                | Enables forwarded headers middleware for proxy support                                                                           | `true`                                       |
| `EnableMsDotNetDataProtectionServices`  | Enables .NET data protection services                                                                                            | `true`                                       |
| `EnableOpenIdCertificates`              | Enables OpenID certificate validation                                                                                            | `true`                                       |
| `EnableOneLoginCertificateRotation`     | Enables OneLogin certificate rotation                                                                                            | `true`                                       |
| `EnableDevelopmentOpenIdCertificates`   | Enables development OpenID certificates                                                                                          | `true`                                       |

### Configuration

Feature flags can be configured in several ways:

1. **Environment-specific configuration files** (e.g., `appsettings.Development.json`, `appsettings.Production.json`)
2. **Environment variables** (prefixed with the configuration section)
3. **Azure App Configuration** (in production environments)
4. **User secrets** (for local development)

Example configuration in `appsettings.Development.json`:

```json
{
  "FeatureFlags": {
    "SupportEndToEndTesting": true,
    "RequiresDbConnection": false,
    "EnableDeveloperExceptionPage": true,
    "EnableMigrationsEndpoint": true,
    "EnableSwagger": true,
    "EnableHttpStrictTransportSecurity": false
  }
}
```

## Formatting

Before committing you can format any changed files by running:

```shell
just format-changed
```

To format the entire codebase run

```shell
just format
```
