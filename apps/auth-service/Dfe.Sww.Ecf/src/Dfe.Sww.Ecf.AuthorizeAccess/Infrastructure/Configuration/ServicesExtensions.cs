using Azure.Identity;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Infrastructure.Configuration;

public static class ServicesExtensions
{
    public static IHostApplicationBuilder AddServiceDefaults(
        this IHostApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks();
        builder.AddDatabase();

        builder.Services.AddHealthChecks().AddNpgSql(builder.Configuration.GetPostgresConnectionString());

        var featureFlags = builder.Services
            .BuildServiceProvider()
            .GetRequiredService<FeatureFlags>();

        if (featureFlags.EnableMigrationsEndpoint) {
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        }

        if (featureFlags.EnableForwardedHeaders) {
            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.All;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });
        }

        if (featureFlags.EnableHttpStrictTransportSecurity) {
            builder.Services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(365);
            });
        }

        if (featureFlags.EnableMsDotNetDataProtectionServices) {
            builder.Services.AddDataProtection()
                .PersistKeysToAzureBlobStorage(
                    // This will use managed identity for auth so the 
                    // connection string will just be a URL to the blob container
                    new Uri(builder.Configuration.GetRequiredValue("StorageConnectionString")),
                    new DefaultAzureCredential()
                );
        }

        return builder;
    }

    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        var featureFlags = app.Services
            .GetRequiredService<FeatureFlags>();

        if (featureFlags.EnableForwardedHeaders)
        {
            app.UseForwardedHeaders();
        }
        if (featureFlags.EnableHttpStrictTransportSecurity)
        {
            app.UseHsts();
        }

        app.MapGet("/health", async context =>
        {
            await context.Response.WriteAsync("OK");
        });

        app.UseHealthChecks("/status");

        return app;
    }
}
