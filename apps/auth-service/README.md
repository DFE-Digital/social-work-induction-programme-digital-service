# SWPDP digital service - Auth service (`auth-service`)

## Installation

See [LOCALDEV one-time installation](/README.md#localdev-one-time-installation) and [Starting LOCALDEV](/README.md#starting-localdev)
for installation instructions for the SWPDP digital service.

## Overview

Auth service (`auth-service`) provides an API for authentication and data retrieval/persistence. It is an ASP.NET Core 8 web application.

## One Login simulator

- TODO: VERIFY On installation, a single admin user is added to the `auth-db` database (the `persons` and `person_roles` tables each have one row added).

## Testing

To generate the test database and execute the migrations against it, run:

```bash
cd ~/swpdp/apps/auth-service
just cli migrate-db --connection TestDbConnection
```

TODO: Document testing

#### Manual Database Updates

##### Post database setup

This section only needs to be done once when creating the database. Once the database migrations have successfully completed you need to add a person into the `person` table.

You can use the following script:

```sql
INSERT INTO public.persons (person_id, created_on, updated_on, deleted_on, trn, first_name, middle_name, last_name, date_of_birth, email_address, national_insurance_number) VALUES ('<GUID_HERE>', NOW(), null, null, '<SOCIAL_WORK_ENGLAND_ID>', '<FIRST_NAME>', ' ', '<LAST_NAME>', '<DATE_OF_BIRTH>', null, null);
```

Once a person has been added you need to link the person record with the record in `one_login_users` (if none are there, try starting the app and logging into OneLogin). You can link the person to the one login user by updating the `person_id` column in the `one_login_users` table to match the record that was just created in the `person` table. Also, given this person is added manually, you need to also add their role in the `person_roles` table.

You also need to create an entry in the `organisations` table and insert a row in the `person_organisations` with the ID of the organisation and ID of the person as the foreign keys `person_id` and `organisation_id`.

##### Applying migrations

If changes have been made in the repo that include migrations, these need to be applied locally. You should be able to use the following command in the terminal while in the Core project:
`dotnet ef database update`

### OneLogin Integration

We integrate with [Gov.UK OneLogin](https://www.sign-in.service.gov.uk/documentation) to provide us authentication services for our users. To utilise the integration locally, there are two methods; the GOV.UK One Login [integration environment](https://docs.sign-in.service.gov.uk/integrate-with-integration-environment/), or the [simulator](https://github.com/govuk-one-login/simulator).

#### Simulator

The [simulator](https://github.com/govuk-one-login/simulator) is a Node application that acts as the OpenID authentication server in place of the real One Login service. It runs entirely locally as a container and allows us full control over what the response is from the auth flow. It also features a [config](https://github.com/govuk-one-login/simulator/blob/main/docs/configuration.md) endpoint that can be used to change the responses of the service on-the-fly. Further information and guides can be found in the official [GitHub repo](https://github.com/govuk-one-login/simulator) for the simulator.

The development configurations for the auth service are already pre-configured to use the simulator by default, all you have to do is generate some local SSL certificates and start it up.
This process has been simplified with the some `just` commands.

Ensure you have [mkcert](https://github.com/FiloSottile/mkcert?tab=readme-ov-file#installation) installed on your machine (to generate the necessary SSL certificates) and run `just onelogin-sim setup`. This will generate the certificates needed, trust them, and place them in the `tools/onelogin-simulator/certs` directory.

After this, you can simply run `just onelogin-sim start` to start the simulator. By default, it will be hosted at https://localhost:3000/onelogin. You can confirm it is working by going to https://localhost:3000/onelogin/config in your browser and you should see the current configuration of the simulator as a JSON object.

`just onelogin-sim stop` can be used to stop the simulator.

Ensure you comment out any local secrets for `OneLogin:ClientId` and `OneLogin:PrivateKeyPem` associated with the One Login integration environment when using the simulator. Secrets take precendence over variables specified in app settings.

Note: The simulator does _not_ emulate the UI of the One Login service; it will not show the user any login pages or forms, it will simply return whatever response it is configured to give. By default, it will log the user in successfully with the default values specified [here](https://github.com/govuk-one-login/simulator).

#### Integration environment

In order to use the official GOV.UK One Login [integration environment](https://docs.sign-in.service.gov.uk/integrate-with-integration-environment/), the application needs to be configured with the API account details in order for this to work.

The endpoint used by the auth service can be changed in the [appsettings.development.json](Dfe.Sww.Ecf\src\Dfe.Sww.Ecf.AuthorizeAccess\appsettings.Development.json). Update the `OneLogin.Url` to point at `https://oidc.integration.account.gov.uk` instead. The `ClientId` and `PrivateKeyPem` will need to be deleted or commented out as they will be specified via user-secrets instead.

The `ClientId` and `PrivateKeyPem` can be configured using user-secrets, the same way we specify database connection details.
The two values to configure are `OneLogin:ClientId` and `OneLogin:PrivateKeyPem`. Speak to the team to acquire these details.

To set these, run the following:

```shell
just set-secret OneLogin:ClientId "exampleClientId"
just set-secret OneLogin:PrivateKeyPem "-----BEGIN PRIVATE KEY-----\nExamplePrivateKeyPem\nWithNewLinesEscaped\nSoItsOnASingleLine\n-----END PRIVATE KEY-----"
```

To run the tests, you will also need to set the OneLogin secrets for the test projects:

```shell
just set-tests-secret OneLogin:ClientId "exampleClientId"
just set-tests-secret OneLogin:PrivateKeyPem "-----BEGIN PRIVATE KEY-----\nExamplePrivateKeyPem\nWithNewLinesEscaped\nSoItsOnASingleLine\n-----END PRIVATE KEY-----"
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
