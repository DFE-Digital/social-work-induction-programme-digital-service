using System.Text;
using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Secrets;
using Dfe.Analytics;
using Dfe.Analytics.AspNetCore;
using Dfe.Sww.Ecf;
using Dfe.Sww.Ecf.AuthorizeAccess;
using Dfe.Sww.Ecf.AuthorizeAccess.Infrastructure.Configuration;
using Dfe.Sww.Ecf.AuthorizeAccess.Infrastructure.Filters;
using Dfe.Sww.Ecf.AuthorizeAccess.Infrastructure.FormFlow;
using Dfe.Sww.Ecf.AuthorizeAccess.Infrastructure.Logging;
using Dfe.Sww.Ecf.AuthorizeAccess.Infrastructure.Security;
using Dfe.Sww.Ecf.AuthorizeAccess.Infrastructure.Security.Configuration;
using Dfe.Sww.Ecf.AuthorizeAccess.TagHelpers;
using Dfe.Sww.Ecf.Core.Services.Accounts;
using Dfe.Sww.Ecf.Core.Services.Organisations;
using Dfe.Sww.Ecf.SupportUi.Infrastructure.FormFlow;
using Dfe.Sww.Ecf.UiCommon.Filters;
using Dfe.Sww.Ecf.UiCommon.FormFlow;
using Dfe.Sww.Ecf.UiCommon.Middleware;
using GovUk.Frontend.AspNetCore;
using JetBrains.Annotations;
using Joonasw.AspNetCore.SecurityHeaders;
using Joonasw.AspNetCore.SecurityHeaders.Csp;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.Configuration;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);
var logger = LoggerFactory.Create(b => b.AddConsole()).CreateLogger<Program>();

logger.LogInformation("Starting auth-service application");

builder.Services.AddOptions<AuthorizeAccessOptions>().Bind(builder.Configuration).ValidateDataAnnotations()
    .ValidateOnStart();
var applicationOptions = builder.Configuration.Get<AuthorizeAccessOptions>();
if (applicationOptions is null)
{
    throw new InvalidConfigurationException("Failed to parse application configuration.");
}

builder.Services.AddOptions<AppInfo>().Bind(builder.Configuration.GetRequiredSection("AppInfo"));

var featureFlagsSection = builder.Configuration.GetRequiredSection(FeatureFlagOptions.ConfigurationKey);
builder.Services.AddOptions<FeatureFlagOptions>().Bind(featureFlagsSection);
var featureFlags = featureFlagsSection.Get<FeatureFlagOptions>();

if (featureFlags is null)
{
    throw new InvalidConfigurationException("Failed to parse feature flags configuration.");
}

if (featureFlags.EnableOneLoginCertificateRotation || featureFlags.EnableOpenIdCertificates)
{
    if (applicationOptions.KeyVaultUri is null)
    {
        throw new InvalidConfigurationException(
            "KeyVaultUri must be set if one of the certificate rotation features are enabled.");
    }

    var keyVaultUri = new Uri(applicationOptions.KeyVaultUri);
    builder.Services
        .AddSingleton(_ => new SecretClient(keyVaultUri, new DefaultAzureCredential()))
        .AddSingleton(_ => new CertificateClient(keyVaultUri, new DefaultAzureCredential()));
}

builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

// Add environment variables settings integration for Azure
builder.Configuration.AddEnvironmentVariables();
builder.AddServiceDefaults();

builder.ConfigureLogging();
builder.Services.AddOptions<OidcOptions>().Bind(builder.Configuration.GetRequiredSection(
    OidcOptions.ConfigurationKey
));

builder.Services.AddGovUkFrontend();
builder.Services.AddCsp();

builder.AddAuthentication(logger);
builder.AddOpenIddict(logger);

builder
    .Services.AddDfeAnalytics()
    .AddAspNetCoreIntegration(options =>
    {
        options.UserIdClaimType = ClaimTypes.Subject;

        options.RequestFilter = ctx =>
            ctx.Request.Path != "/status"
            && ctx.Request.Path != "/health"
            && ctx.Features.Any(f => f.Key == typeof(IEndpointFeature));
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swaggerGenOptions =>
{
    swaggerGenOptions.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "Accounts API",
            Version = "v1",
            Description = "API for managing accounts in the system"
        }
    );
    // If you need to add authentication
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT Bearer token **_only_**"
    };
    swaggerGenOptions.AddSecurityDefinition("Bearer", securityScheme);
    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            []
        }
    };
    swaggerGenOptions.AddSecurityRequirement(securityRequirement);
});
builder
    .Services.AddOptions<SwaggerUIOptions>()
    .Configure<IHttpContextAccessor>((swaggerUiOptions, httpContextAccessor) =>
        {
            // 2. Take a reference of the original Stream factory which reads from Swashbuckle's embedded resources
            var originalIndexStreamFactory = swaggerUiOptions.IndexStream;
            // 3. Override the Stream factory
            swaggerUiOptions.IndexStream = () =>
            {
                // 4. Read the original index.html file
                using var originalStream = originalIndexStreamFactory();
                using var originalStreamReader = new StreamReader(originalStream);
                var originalIndexHtmlContents = originalStreamReader.ReadToEnd();
                // 5. Get the request-specific nonce generated by NetEscapades.AspNetCore.SecurityHeaders
                var requestSpecificNonce = httpContextAccessor
                    .HttpContext?.RequestServices.GetRequiredService<ICspNonceService>()
                    .GetNonce();
                // 6. Replace inline `<script>` and `<style>` tags by adding a `nonce` attribute to them
                var nonceEnabledIndexHtmlContents = originalIndexHtmlContents
                    .Replace(
                        "<script>",
                        $"<script nonce=\"{requestSpecificNonce}\">",
                        StringComparison.OrdinalIgnoreCase
                    )
                    .Replace(
                        "<style>",
                        $"<style nonce=\"{requestSpecificNonce}\">",
                        StringComparison.OrdinalIgnoreCase
                    );
                // 7. Return a new Stream that contains our modified contents
                return new MemoryStream(Encoding.UTF8.GetBytes(nonceEnabledIndexHtmlContents));
            };
        }
    );

builder
    .Services.AddRazorPages()
    .AddMvcOptions(options =>
    {
        options.Filters.Add(new NoCachePageFilter());
        options.Filters.Add(new AssignViewDataFromFormFlowJourneyResultFilterFactory());
    });

if (featureFlags.RequiresDbConnection)
{
    builder.Services.AddDbContext<IdDbContext>(options =>
        options
            .UseNpgsql(builder.Configuration.GetRequiredConnectionString("DefaultConnection"))
            .UseOpenIddict<Guid>()
    );
}

builder.Services
    .AddEcfBaseServices()
    .AddTransient<AuthorizeAccessLinkGenerator, RoutingAuthorizeAccessLinkGenerator>()
    .AddTransient<FormFlowJourneySignInHandler>()
    .AddTransient<MatchToEcfAccountAuthenticationHandler>()
    .AddHttpContextAccessor()
    .AddSingleton<IStartupFilter, FormFlowSessionMiddlewareStartupFilter>()
    .AddFormFlow(options => { options.JourneyRegistry.RegisterJourney(SignInJourneyState.JourneyDescriptor); })
    .AddSingleton<ICurrentUserIdProvider, FormFlowSessionCurrentUserIdProvider>()
    .AddTransient<SignInJourneyHelper>()
    .AddSingleton<ITagHelperInitializer<FormTagHelper>, FormTagHelperInitializer>()
    .AddHostedService<OidcApplicationSeeder>()
    .AddScoped<IAccountsService, AccountsService>()
    .AddScoped<IOrganisationService, OrganisationService>()
    .AddScoped<IOneLoginAccountLinkingService, OneLoginAccountLinkingService>()
    .AddSingleton(sp => sp.GetRequiredService<IOptions<AppInfo>>().Value);

builder.AddTestApp();

var app = builder.Build();

logger.LogInformation("Application built, configuring middleware");

app.MapDefaultEndpoints();

app.UseWhen(
    context => !context.Request.Path.StartsWithSegments("/oauth2"),
    a =>
    {
        if (featureFlags.EnableDeveloperExceptionPage)
        {
            a.UseDeveloperExceptionPage();
        }

        if (featureFlags.EnableMigrationsEndpoint)
        {
            a.UseMigrationsEndPoint();
        }

        if (featureFlags.EnableErrorExceptionHandler)
        {
            a.UseExceptionHandler("/error");
            a.UseStatusCodePagesWithReExecute("/error", "?code={0}");
        }
    }
);

app.UseCsp(csp =>
{
    var pageTemplateHelper = app.Services.GetRequiredService<PageTemplateHelper>();
    var cspSettings = builder.Configuration.GetSection("ContentSecurityPolicy");
    var scriptHash = cspSettings["ScriptHash"];

    csp.ByDefaultAllow.FromSelf();

    csp.AllowScripts.FromSelf().From(pageTemplateHelper.GetCspScriptHashes()).AddNonce();

    csp.AllowScripts.AllowUnsafeInline().WithHash(scriptHash);

    csp.AllowStyles.FromSelf().AddNonce();

    // Ensure ASP.NET Core's auto refresh works
    // See https://github.com/dotnet/aspnetcore/issues/33068
    if (featureFlags.EnableContentSecurityPolicyWorkaround)
    {
        csp.AllowConnections.ToAnywhere();
    }
});

app.UseMiddleware<AppendSecurityResponseHeadersMiddleware>();

app.UseStaticFiles();

if (featureFlags.EnableDfeAnalytics)
{
    app.UseDfeAnalytics();
}

if (featureFlags.EnableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

logger.LogInformation("Application starting");

app.Run();

logger.LogInformation("Application stopped");

[PublicAPI]
public partial class Program
{
}
