# social-worker-workforce-early-careers-framework

Note: Any diagrams in .jpg files can be rendered using the [.dsl files and https://structurizr.com/dsl](docs/c4-diagrams-as-code/)

## Developer setup

### Software requirements

The API is an ASP.NET Core 8 web application. To develop locally you will need the following installed:

- Visual Studio 2022 (or the .NET 8 SDK and an alternative IDE/editor);
- a local Postgres 13+ instance.
- [SASS](https://sass-lang.com/install).

A `justfile` defines various recipes for development. Ensure [just](https://just.systems/) is installed and available on your `$PATH` as well as [PowerShell](https://microsoft.com/PowerShell).
Note: 'Windows Powershell' and 'Powershell' are two different things! Powershell is the cross-platform version that works on any OS.
Please ensure you have 'Powershell' installed.
To setup PowerShell on a MacOS machine, use you can follow [this guide](https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell-on-macos?view=powershell-7.4). For Windows, you can follow [this guide](https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell-on-windows?view=powershell-7.4).

### Setup

- `just install-tools`
- `just restore`
- `just build`

### Database

The auth service utilises a PostgreSQL database to store it's data. An existing PostgreSQL instance can be used by the service when running locally or a new instance can be created and run in a docker container for development use.

#### PostgreSQL in a docker container using Podman

[Podman](https://podman.io) is an open source alternative to Docker Desktop and doesn't require a commercial licence for us to use. It is almost a 1:1 drop-in replacement for Docker Desktop and it's CLI commands/arguments are essentially the same. So if you don't use Podman on your machine, a container host of your choice that uses standard Docker commands should work the same.
If you haven't already got Podman installed and configured on your machine, please follow their documentation on how to do that [here](https://podman.io/get-started).

To get a local instance of PostgreSQL running in a container using Podman, you can pull the latest `postgres` image from the Docker hub and run it like so:

```zsh
podman pull postgres
podman run --name auth-db -e POSTGRES_USER=<DB_USER> -e POSTGRES_PASSWORD=<DB_PASSWORD> -p 5432:5432 -v <DB_DATADIR> -d postgres
```

Be sure to replace any placeholder strings (`< ... >`) with values specific to your environment.
The `DB_USER` and `DB_PASSWORD` variables will be utilised by the local auth service instance so be sure to make a note of them to be used when configuring the service. Mapping a volume to a `DB_DATADIR` is an optional step but will ensure your database is persisted through restarts.

#### Database migrations

Once your PostgreSQL instance is up and running, you can run the migrations to create the required database and table structure for the auth service.

Setup a connection string configuration entry in user secrets for running the apps and another for running the tests.
Note you should use a different database for tests as the test database will be cleared down whenever tests are run.
There are `just` commands defined to help set the correct user-secrets for you:

```shell
just set-db-connection "Host=localhost;Username=<DB_USER>;Password=<DB_PASSWORD>;Database=sww-ecf"
just set-test-db-connection "Host=localhost;Username=<TEST_DB_USER>;Password=<TEST_DB_PASSWORD>;Database=sww-ecf_tests"
```

To execute the migrations and set up the initial database schema run:

```shell
just build
just cli migrate-db
```

To generate the test database and execute the migrations against it, run:

```shell
just cli migrate-db --connection TestDbConnection
```

Run `just cli migrate-db -h` for more information on the `migrate-db` command.
You can also run `just ef ...` to invoke the dotnet entity framework CLI directly.

### OneLogin Integration

We integrate with [Gov.UK OneLogin](https://www.sign-in.service.gov.uk/documentation) to provide us authentication services for our users. The application needs to be configured with the API account details in order for this to work.

These can be configured using user-secrets, the same way we specify database connection details.
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

## Formatting

Before committing you can format any changed files by running:

```shell
just format-changed
```

To format the entire codebase run

```shell
just format
```
