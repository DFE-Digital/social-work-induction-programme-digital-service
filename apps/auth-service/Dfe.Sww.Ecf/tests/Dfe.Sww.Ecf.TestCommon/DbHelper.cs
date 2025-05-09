using System.Data.Common;
using System.Security.Cryptography;
using System.Text;
using Dfe.Sww.Ecf.Core.DataStore.Postgres;
using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using SystemUser = Dfe.Sww.Ecf.Core.DataStore.Postgres.Models.SystemUser;

namespace Dfe.Sww.Ecf.TestCommon;

public class DbHelper(string connectionString)
{
    private Respawner? _respawner;
    private readonly SemaphoreSlim _schemaLock = new(1, 1);
    private bool _haveResetSchema = false;

    public string ConnectionString { get; } = connectionString;

    public static void ConfigureDbServices(IServiceCollection services, string connectionString)
    {
        services.AddDbContext<EcfDbContext>(
            options => EcfDbContext.ConfigureOptions(options, connectionString),
            contextLifetime: ServiceLifetime.Transient
        );

        services.AddDbContextFactory<EcfDbContext>(options =>
            EcfDbContext.ConfigureOptions(options, connectionString)
        );

        services.AddSingleton(new DbHelper(connectionString));

        services.AddStartupTask(sp => sp.GetRequiredService<DbHelper>().EnsureSchema());
    }

    public async Task ClearData()
    {
        using var dbContext = EcfDbContext.Create(ConnectionString);
        await dbContext.Database.OpenConnectionAsync();
        var connection = dbContext.Database.GetDbConnection();
        await EnsureRespawner(connection);
        await _respawner!.ResetAsync(connection);

        // Ensure we have the System User around
        dbContext.Set<SystemUser>().Add(SystemUser.Instance);
        var roles = Enum.GetValues(typeof(RoleType))
            .Cast<RoleType>()
            .Select(roleType => new Role { RoleId = (int)roleType, RoleName = roleType })
            .ToArray();
        dbContext.Set<Role>().AddRange(roles);
        await dbContext.SaveChangesAsync();
    }

    public async Task EnsureSchema()
    {
        await _schemaLock.WaitAsync();

        try
        {
            if (!_haveResetSchema)
            {
                await ResetSchema();
                _haveResetSchema = true;
            }
        }
        finally
        {
            _schemaLock.Release();
        }
    }

    public async Task ResetSchema()
    {
        using var dbContext = EcfDbContext.Create(ConnectionString);

        var connection = dbContext.Database.GetDbConnection();
        var dbName = connection.Database;

        var cachedMigrationsVersionPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Dfe.Sww.Ecf.Tests",
            $"{dbName}-dbversion.txt"
        );

        var currentDbVersion = Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(dbContext.Database.GenerateCreateScript()))
        );

        if (currentDbVersion == GetPreviousMigrationsVersion())
        {
            await ClearData();
            return;
        }

        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.MigrateAsync();

        WriteMigrationsVersion();

        await connection.OpenAsync();
        await EnsureRespawner(connection);

        string? GetPreviousMigrationsVersion() =>
            File.Exists(cachedMigrationsVersionPath)
                ? File.ReadAllText(cachedMigrationsVersionPath)
                : null;

        void WriteMigrationsVersion()
        {
            var directory = Path.GetDirectoryName(cachedMigrationsVersionPath)!;
            Directory.CreateDirectory(directory);
            File.WriteAllText(cachedMigrationsVersionPath, currentDbVersion);
        }
    }

    private async Task EnsureRespawner(DbConnection connection) =>
        _respawner = await Respawner.CreateAsync(
            connection,
            new RespawnerOptions()
            {
                DbAdapter = DbAdapter.Postgres,
                TablesToIgnore =
                [
                    "mandatory_qualification_providers",
                    "establishment_sources",
                    "tps_establishment_types",
                ],
            }
        );
}
