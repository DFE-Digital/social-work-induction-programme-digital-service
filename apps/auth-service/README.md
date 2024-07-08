# social-worker-workforce-early-careers-framework
Note: Any diagrams in .jpg files can be rendered using the [.dsl files and https://structurizr.com/dsl](docs/c4-diagrams-as-code/)

## Developer setup

### Software requirements

The API is an ASP.NET Core 8 web application. To develop locally you will need the following installed:
- Visual Studio 2022 (or the .NET 8 SDK and an alternative IDE/editor);
- a local Postgres 13+ instance.
- [SASS]( https://sass-lang.com/install).

A `justfile` defines various recipes for development. Ensure [just](https://just.systems/) is installed and available on your `$PATH` as well as [PowerShell](https://microsoft.com/PowerShell).

If you're working on infrastructure you will also need:
- make;
- Terraform;
- bash.

### Local tools setup

dotnet-format is required for linting. A `just` recipe will install it:
```shell
just install-tools
```

### Database setup

Install Postgres then set a connection string configuration entry in user secrets for running the apps and another for running the tests.
Note you should use a different database for tests as the test database will be cleared down whenever tests are run.

e.g.
```shell
just set-secret ConnectionStrings:DefaultConnection "Host=localhost;Username=postgres;Password=your_postgres_password;Database=sww-ecf"
just set-tests-secret ConnectionStrings:DefaultConnection "Host=localhost;Username=postgres;Password=your_postgres_password;Database=sww-ecf_tests"
```

To set up the initial database schema run:
```shell
just build
just cli migrate-db
```

The databases will be created automatically when running the tests.

### Admin user setup

Add yourself to your local database as an administrator:
```shell
just create-admin "your.name@education.gov.uk" "Your Name"
```

## Environment configuration

Environment-specific configuration is stored in Key Vault inside a single JSON-encoded key named 'AppConfig'.
This is retrieved via Terraform at deployment time and is set as an environment variable so it can be accessed by the application.


## Formatting

Pull request builds will run format checks on .NET and Terraform code changes; if there are any issues the build will fail.

Before committing you can format any changed files by running:
```shell
just format-changed
```

To format the entire codebase run
```shell
just format
```
