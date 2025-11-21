# Social Work Practice Development Programme Digital Service - User Management Front End

## Setup

App settings will need to be provided to run the application locally. You will need to provide values for `SocialWorkEnglandClientOptions`. The required values are listed below.

### Social Work England Client Settings

e.g.

```json
"SocialWorkEnglandClientOptions": {
    "BaseUrl": "",
    "Routes": {
      "SocialWorker":{
        "GetById": ""
      }
    }
  }
```

You will also need to provide values for the SWE API HTTP Client in a **.Net User Secrets** file. **DO NOT PROVIDE THESE IN THE APP SETTINGS FILE**

You will need to provide values for `SocialWorkEnglandClientOptions` secrets. The required values are listed below.

e.g.

```json
{
    "SocialWorkEnglandClientOptions:ClientCredentials:ClientId": "",
    "SocialWorkEnglandClientOptions:ClientCredentials:ClientSecret": "",
    "SocialWorkEnglandClientOptions:ClientCredentials:AccessTokenUrl": ""
}
```

### Moodle Service Client Settings

```json
"MoodleClientOptions": {
    "BaseUrl": "https://moodle.ddev.site/webservice/rest/server.php"
},
```

You will also need to provide values for the Moodle Client in a **.Net User Secrets** file. **DO NOT PROVIDE THESE IN THE APP SETTINGS FILE**.
You can obtain a new API key by following the `Create a Moodle a web service` section in the Moodle README. The README can be found here `social-work-induction-programme-digital-service\README.md`

```json
"MoodleClientOptions:ApiToken": "{API_KEY}"
```

## Feature Flags

The user management frontend uses feature flags to control various functionality. Feature flags are configured in the `appsettings.json` files and can be overridden using environment variables or other configuration sources.

### Available Feature Flags

| Flag                                    | Description                                                                                                                                                                               | Default Value                                |
| --------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | -------------------------------------------- |
| `EnableDeveloperExceptionPage`          | Provides detailed error information and stack traces during development to aid debugging                                                                                                  | `true` in Development, `false` in Production |
| `EnableHttpStrictTransportSecurity`     | Enables HSTS security headers                                                                                                                                                             | `false` in Development, `true` in Production |
| `EnableContentSecurityPolicyWorkaround` | Enables Content Security Policy workaround for ASP.NET Core auto refresh (see [dotnet/aspnetcore#33068](https://github.com/dotnet/aspnetcore/issues/33068))                               | `true` in Development, `false` in Production |
| `EnableForwardedHeaders`                | Enables forwarded headers middleware for proxy support                                                                                                                                    | `true`                                       |
| `EnableMoodleIntegration`               | Enables Moodle learning management system integration functionality                                                                                                                       | `true`                                       |
| `EnablePlusEmailStripping`              | Enables removal of 'plus tags' (See [RFC 5233](https://www.rfc-editor.org/rfc/rfc5233) on Subaddress Extensions) from email addresses before sending emails via the Notification service. | `true`                                       |

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
        "EnableDeveloperExceptionPage": true,
        "EnableHttpStrictTransportSecurity": false,
        "EnableMoodleIntegration": true,
        "EnablePlusEmailStripping": true
    }
}
```

## Tasks

### Running Tests

To run Frontend Tests locally use `pnpm playwright test --project=frontend` after starting the auth service, the One Login simulator and the frontend app. 
Note: `INTERACTIVE_MODE=true` needs to be set in `auth-service/tools/onelogin-simulator/compose.yml`.

To run API Tests use- `pnpm playwright test --project=swe_api`

### Running Performance Tests

Prerequisites: at least Java v8+ installed

- cd to `/apps/swe-performance-test`
- `dotnet restore`

Set the following secrets:

- `dotnet user-secrets set 'ClientId' 'id'`
- `dotnet user-secrets set 'ClientSecret' 'secret'`
- `dotnet user-secrets set 'AccessTokenURL' 'url'`

Set the following environment variables:

- `export ENVIRONMENT=preprod` or `export ENVIRONMENT=production` (or if you are on Windows `set ENVIRONMENT=preprod` or `set ENVIRONMENT=production` or using powershell `set-item -path env:ENVIRONMENT -value preprod` or `set-item -path env:ENVIRONMENT -value production`)

Run the test:

- `dotnet test --logger "Console;verbosity=normal"`

Reports are saved to the `dist/apps/swe-performance-test/net8.0/performance-test-results` folder.
