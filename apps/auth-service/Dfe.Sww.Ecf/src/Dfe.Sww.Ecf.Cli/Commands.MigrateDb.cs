using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Dfe.Sww.Ecf.Core.DataStore.Postgres;

namespace Dfe.Sww.Ecf.Cli;

public static partial class Commands
{
    public static Command CreateMigrateDbCommand(IConfiguration configuration)
    {
        const string defaultConnectionKey = "DefaultConnection";
        const string testDbConnectionKey = "TestDbConnection";

        var connectionOption = new Option<string>("--connection", () => defaultConnectionKey,
            $"The name of the ConnectionString stored in user-secrets to use for migrations. Example: '{testDbConnectionKey}'.");
        connectionOption.AddCompletions([defaultConnectionKey, testDbConnectionKey]);
        var connectionStringOption = new Option<string?>("--connection-string",
            "The connection string of the database to run migrations against. Will override '--connection' when specified.");
        var targetMigrationOption = new Option<string?>("--target-migration",
            "The target migration to migrate up to. Defaults to the latest migration.");

        var migrateDbCommand = new Command("migrate-db", "Migrate the database to the latest version.")
        {
            connectionOption,
            connectionStringOption,
            targetMigrationOption
        };

        migrateDbCommand.SetHandler(
            async (connection, connectionString, targetMigration) =>
                await MigrateDb(configuration, connection, connectionString, targetMigration),
            connectionOption,
            connectionStringOption,
            targetMigrationOption);

        return migrateDbCommand;
    }

    private static async Task MigrateDb(IConfiguration configuration, string connection, string? connectionString,
        string? targetMigration)
    {
        connectionString ??= configuration.GetConnectionString(connection);
        if (connectionString is null)
        {
            throw new ArgumentException("Connection not specified or configured.");
        }

        await using var dbContext =
            EcfDbContext.Create(connectionString, (int)TimeSpan.FromMinutes(10).TotalSeconds);
        await dbContext.GetService<IMigrator>().MigrateAsync(targetMigration);
    }
}
