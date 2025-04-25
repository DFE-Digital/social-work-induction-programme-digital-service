# Social Worker Workforce - Early Career Framework - Front End

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
