using Dfe.Sww.Ecf.AuthorizeAccess.Infrastructure.Configuration;
using Serilog;


namespace Dfe.Sww.Ecf.AuthorizeAccess.Infrastructure.Logging;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder ConfigureLogging(this WebApplicationBuilder builder)
    {
        var featureFlags = builder.Services
            .BuildServiceProvider()
            .GetRequiredService<FeatureFlags>();

        if (featureFlags.EnableSentry)
        {
            builder.WebHost.UseSentry(dsn: builder.Configuration.GetRequiredValue("Sentry:Dsn"));
        }

        builder.Services.AddApplicationInsightsTelemetry();

        // We want all logging to go through Serilog so that our filters are always applied
        builder.Logging.ClearProviders();

        builder.Host.UseSerilog((ctx, services, config) => config.ConfigureSerilog(ctx.HostingEnvironment, ctx.Configuration, services));

        return builder;
    }
}
