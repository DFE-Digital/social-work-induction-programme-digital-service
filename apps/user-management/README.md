# Social Worker Workforce - Early Career Framework

## Information
This is the root README which contains information that is relevant to the entire repo. Please read the application/project specific README's for information relating to individual applications

## Development

### Coding Conventions & Standards
#### CSharpier
We apply coding standards using [CSharpier](https://csharpier.com/) and `.editorconfig`. In order to apply the formatting on save you can install the extension for your chosen IDE [here](https://csharpier.com/docs/Editors), and follow the instructions in the extension overview.

#### Extension Links
- [Visual Studio 2022](https://marketplace.visualstudio.com/items?itemName=csharpier.CSharpier) - `Configure Reformat with CSharpier on Save under Tools | Options | CSharpier | General`
- [JetBrains Rider](https://plugins.jetbrains.com/plugin/18243-csharpier) - `Configure CSharpier to Run on Save under Preferences/Settings | Tools | CSharpier`

### Tooling and Package Management

#### Tool versions (`asdf`)
This repo has been setup to use [asdf](https://asdf-vm.com) to manage tooling versions. Follow their [Getting Started](https://asdf-vm.com/guide/getting-started.html) guide to install `asdf` on your machine.

#### Package manager (`pnpm`)
For package management, we use [pnpm](https://pnpm.io). Once you have installed `asdf`, you can setup `pnpm` by simply adding the required plugins and installing it:
```shell
asdf plugin add nodejs
asdf plugin add pnpm
asdf plugin add dotnet
asdf install
```

### Setup

- Install dependencies: `pnpm i`
- Install playwright dependencies: `pnpx playwright install`

### Tasks

###### Running Tests

To run API Tests use- `pnpm playwright test --project=swe_api`
To run Frontend Tests use- `pnpm playwright test --project=frontend`


###### Running Performance Tests

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
