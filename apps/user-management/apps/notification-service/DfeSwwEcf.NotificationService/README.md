# Notification Function 

This repository contains an Azure Function that allows us to send notificationn through . The function is built using .NET Core and is triggered by an HTTP request.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Configuration](#configuration)
- [Usage](#usage)
- [Testing](#testing)
- [Contributing](#contributing)

## Prerequisites

Before you begin, ensure you have the following installed on your machine:

- [.NET Core SDK](https://dotnet.microsoft.com/download/dotnet-core)
- [Azure Functions Core Tools](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local)
- [Visual Studio Code](https://code.visualstudio.com/) or any other IDE of your choice (e.g. Visual Studio 2022 / JetBrains Rider)
- [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)

## Installation

1. Clone the repository:

    ```bash
    git clone https://github.com/DFE-Digital/csc-social-work-ecf-digital.git
    cd apps\notification-function
    ```

2. Install the required dependencies:

    ```bash
    dotnet restore
    ```

## Configuration

1. Create a `local.settings.json` file in the root of the project directory. This file is used for local development and should not be committed to source control. Add the following configuration:

    ```json
    {
      "IsEncrypted": false,
      "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated"
      }
    }
    ```

2. Create a `secrets.json` file. This file is used to store configuration values that shouldn't be commited to source control. Add the following configuration: 

    ```json
    {
        "GovNotify": {
            "ApiKey": "api-key"
        }
    }
    ```
3. Update the configuration values as needed.


## Usage

1. Start the Azure Function locally:

    ```bash
    func start
    ```

2. The function will be available at `http://localhost:7071/api/Notification`. You can trigger it by sending an HTTP request using tools like `curl`, Postman, or your web browser.

3. You will need to supply the following request body. Update the values as needed.

    ```json
    {
        "EmailAddress": "test-email@test.com",
        "TemplateId": "template id here",
        "Personalisation": {
            "personal_text": "ABC123"
        }
    }
    ```

## Testing

To run the tests, use the following command:

```bash
dotnet test
```

## Contributing

Contributions are welcome! Please open an issue or submit a pull request.

1. Fork the repository
2. Create a new branch (git checkout -b feature/your-feature)
3. Make your changes
4. Commit your changes (git commit -m 'Add some feature')
5. Push to the branch (git push origin feature/your-feature)
6. Open a pull request
