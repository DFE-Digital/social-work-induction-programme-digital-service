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

## Feature Flags

The user management frontend uses feature flags to control various functionality. Feature flags are configured in the `appsettings.json` files and can be overridden using environment variables or other configuration sources.

### Available Feature Flags

| Flag                                    | Description                                                                                                                                                                               | Default Value                                |
|-----------------------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|----------------------------------------------|
| `EnableDeveloperExceptionPage`          | Provides detailed error information and stack traces during development to aid debugging                                                                                                  | `true` in Development, `false` in Production |
| `EnableHttpStrictTransportSecurity`     | Enables HSTS security headers                                                                                                                                                             | `false` in Development, `true` in Production |
| `EnableContentSecurityPolicyWorkaround` | Enables Content Security Policy workaround for ASP.NET Core auto refresh (see [dotnet/aspnetcore#33068](https://github.com/dotnet/aspnetcore/issues/33068))                               | `true` in Development, `false` in Production |
| `EnableForwardedHeaders`                | Enables forwarded headers middleware for proxy support                                                                                                                                    | `true`                                       |
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
        "EnablePlusEmailStripping": true
    }
}
```
