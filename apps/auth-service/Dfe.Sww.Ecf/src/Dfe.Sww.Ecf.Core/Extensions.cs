using Dfe.Sww.Ecf.Core.DataStore.Postgres;
using Dfe.Sww.Ecf.Core.Infrastructure.Configuration;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Serilog;
using Serilog.Formatting.Compact;

namespace Dfe.Sww.Ecf.Core;

public static class Extensions
{
    public static IHostApplicationBuilder AddDatabase(this IHostApplicationBuilder builder)
    {
        var pgConnectionString = GetPostgresConnectionString(builder.Configuration);

        var databaseSeedOptions = builder.Configuration.GetSection("DatabaseSeed").Get<DatabaseSeedOptions>();

        builder.Services.AddDbContext<EcfDbContext>(
            options => EcfDbContext.ConfigureOptions(options, pgConnectionString, null, databaseSeedOptions),
            ServiceLifetime.Scoped,
            ServiceLifetime.Singleton
        );

        builder.Services.AddDbContextFactory<EcfDbContext>(options =>
            EcfDbContext.ConfigureOptions(options, pgConnectionString, null, databaseSeedOptions)
        );

        return builder;
    }

    public static void ConfigureSerilog(
        this LoggerConfiguration config,
        IHostEnvironment environment,
        IConfiguration configuration,
        IServiceProvider services
    )
    {
        config
            .ReadFrom.Configuration(configuration)
            .WriteTo.ApplicationInsights(
                services.GetRequiredService<TelemetryConfiguration>(),
                TelemetryConverter.Traces
            )
            .WriteTo.Sentry(o => o.InitializeSdk = false);

        if (environment.IsProduction())
        {
            config.WriteTo.Console(new CompactJsonFormatter());
        }
        else
        {
            config.WriteTo.Console();
        }
    }

    public static string GetPostgresConnectionString(this IConfiguration configuration)
    {
        return new NpgsqlConnectionStringBuilder(
            configuration.GetRequiredValue("ConnectionStrings:DefaultConnection")
        )
        {
            // We rely on error details to get the offending duplicate key values in the TrsDataSyncHelper
            IncludeErrorDetail = true
        }.ConnectionString;
    }
}
