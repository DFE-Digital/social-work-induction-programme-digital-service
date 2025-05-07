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

Note: The simulator does *not* emulate the UI of the One Login service; it will not show the user any login pages or forms, it will simply return whatever response it is configured to give. By default, it will log the user in successfully with the default values specified [here](https://github.com/govuk-one-login/simulator).

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

## Formatting

Before committing you can format any changed files by running:

```shell
just format-changed
```

To format the entire codebase run

```shell
just format
```

## Project background

**In Q4 of 2024, the Early Career Framework (ECF team) was renamed to Social Work Induction Programme (SWIP). The framework is referred to as ‘Post Qualifying Standards’.**

The SWIP Digital project team operates within the Social Work Workforce programme, which also includes the Support for Social Workers. We are part of the digital Children and Families portfolio of the Customer Experience and Digital directorate in DfE.

Our portfolio collaborates with policy teams from the Families and Early Years groups, while other portfolios engage with Schools or Further Education groups. Our primary liaison is with the Children’s Social Care policy team, specifically in the Workforce division.
