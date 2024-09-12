# Social Worker Workforce - Early Career Framework - Front End

## Setup
App settings will need to be provided to run the application locally. You will need to provide values for `SocialWorkEnglandClientOptions`. The required values are listed below.

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