using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
using Dfe.Analytics;
using Dfe.Analytics.AspNetCore;
using Dfe.Sww.Ecf;
using Dfe.Sww.Ecf.AuthorizeAccess;
using Dfe.Sww.Ecf.AuthorizeAccess.Infrastructure.Filters;
using Dfe.Sww.Ecf.AuthorizeAccess.Infrastructure.FormFlow;
using Dfe.Sww.Ecf.AuthorizeAccess.Infrastructure.Logging;
using Dfe.Sww.Ecf.AuthorizeAccess.Infrastructure.Security;
using Dfe.Sww.Ecf.AuthorizeAccess.Infrastructure.Security.Configuration;
using Dfe.Sww.Ecf.AuthorizeAccess.TagHelpers;
using Dfe.Sww.Ecf.Core.DataStore.Postgres;
using Dfe.Sww.Ecf.Core.Infrastructure;
using Dfe.Sww.Ecf.Core.Services.Accounts;
using Dfe.Sww.Ecf.Core.Services.Files;
using Dfe.Sww.Ecf.Core.Services.PersonMatching;
using Dfe.Sww.Ecf.ServiceDefaults;
using Dfe.Sww.Ecf.SupportUi.Infrastructure.FormFlow;
using Dfe.Sww.Ecf.UiCommon.Filters;
using Dfe.Sww.Ecf.UiCommon.FormFlow;
using Dfe.Sww.Ecf.UiCommon.Middleware;
using GovUk.Frontend.AspNetCore;
using GovUk.OneLogin.AspNetCore;
using JetBrains.Annotations;
using Joonasw.AspNetCore.SecurityHeaders;
using Joonasw.AspNetCore.SecurityHeaders.Csp;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.IdentityModel.Protocols.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using static OpenIddict.Abstractions.OpenIddictConstants;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

builder.AddServiceDefaults(dataProtectionBlobName: "AuthorizeAccess");

builder.ConfigureLogging();
var oidcConfigurationSection = builder.Configuration.GetRequiredSection(
    OidcConfiguration.ConfigurationKey
);
var oidcConfiguration = oidcConfigurationSection.Get<OidcConfiguration>();
if (oidcConfiguration is null)
{
    throw new InvalidConfigurationException("Missing OIDC configuration.");
}
if (oidcConfiguration.Issuer is null)
{
    throw new InvalidConfigurationException("OIDC Issuer not configured.");
}
builder.Services.Configure<OidcConfiguration>(oidcConfigurationSection);

builder.Services.AddGovUkFrontend();
builder.Services.AddCsp(nonceByteAmount: 32);

builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = AuthenticationSchemes.MatchToEcfAccount;
        options.DefaultChallengeScheme = AuthenticationSchemes.MatchToEcfAccount;

        options.AddScheme(
            AuthenticationSchemes.FormFlowJourney,
            scheme =>
            {
                scheme.HandlerType = typeof(FormFlowJourneySignInHandler);
            }
        );

        options.AddScheme(
            AuthenticationSchemes.MatchToEcfAccount,
            scheme =>
            {
                scheme.HandlerType = typeof(MatchToEcfAccountAuthenticationHandler);
            }
        );
    })
    .AddOneLogin(options =>
    {
        options.SignInScheme = AuthenticationSchemes.FormFlowJourney;

        options.Events.OnRedirectToIdentityProviderForSignOut = context =>
        {
            // The standard sign out process will call Authenticate() on SignInScheme then try to extract the id_token from the Principal.
            // That won't work in our case most of the time since sign out journeys won't have the FormFlow instance around that has the AuthenticationTicket.
            // Instead, we'll get it passed to us in explicitly in AuthenticationProperties.Items.

            if (
                context.ProtocolMessage.IdTokenHint is null
                && context.Properties.Parameters.TryGetValue(
                    OpenIdConnectParameterNames.IdToken,
                    out var idToken
                )
                && idToken is string idTokenString
            )
            {
                context.ProtocolMessage.IdTokenHint = idTokenString;
            }

            return Task.CompletedTask;
        };

        options.Events.OnAccessDenied = async context =>
        {
            // This handles the scenario where we've requested ID verification but One Login couldn't do it.

            if (
                context.Properties!.TryGetVectorOfTrust(out var vtr)
                && vtr == SignInJourneyHelper.AuthenticationAndIdentityVerificationVtr
                && TryGetJourneyInstanceId(context.Properties, out var journeyInstanceId)
            )
            {
                context.HandleResponse();

                var signInJourneyHelper =
                    context.HttpContext.RequestServices.GetRequiredService<SignInJourneyHelper>();
                var journeyInstance = (
                    await signInJourneyHelper.UserInstanceStateProvider.GetSignInJourneyInstanceAsync(
                        context.HttpContext,
                        journeyInstanceId
                    )
                )!;

                var result = await signInJourneyHelper.OnVerificationFailed(journeyInstance);
                await result.ExecuteAsync(context.HttpContext);
            }
        };

        options.MetadataAddress =
            "https://oidc.integration.account.gov.uk/.well-known/openid-configuration";
        options.ClientAssertionJwtAudience = "https://oidc.integration.account.gov.uk/token";

        using (var rsa = RSA.Create())
        {
            var privateKeyPem = builder.Configuration["OneLogin:PrivateKeyPem"];
            rsa.ImportFromPem(privateKeyPem);

            options.ClientAuthenticationCredentials = new SigningCredentials(
                new RsaSecurityKey(rsa.ExportParameters(includePrivateParameters: true)),
                SecurityAlgorithms.RsaSha256
            );
        }

        options.ClientId =
            builder.Configuration["OneLogin:ClientId"]
            ?? throw new InvalidOperationException("Missing OneLogin:ClientId");
        options.CallbackPath = "/_onelogin/callback";
        options.SignedOutCallbackPath = "/_onelogin/logout-callback";

        options.CorrelationCookie.Name = "onelogin-correlation.";
        options.NonceCookie.Name = "onelogin-nonce.";
        return;

        static bool TryGetJourneyInstanceId(
            AuthenticationProperties? properties,
            [NotNullWhen(true)] out JourneyInstanceId? journeyInstanceId
        )
        {
            if (
                properties?.Items.TryGetValue(
                    FormFlowJourneySignInHandler.PropertyKeys.JourneyInstanceId,
                    out var serializedInstanceId
                ) == true
                && serializedInstanceId is not null
            )
            {
                journeyInstanceId = JourneyInstanceId.Deserialize(serializedInstanceId);
                return true;
            }

            journeyInstanceId = default;
            return false;
        }
    });

builder
    .Services.AddOpenIddict()
    .AddCore(options =>
    {
        options
            .UseEntityFrameworkCore()
            .UseDbContext<EcfDbContext>()
            .ReplaceDefaultEntities<Guid>();
    })
    .AddServer(options =>
    {
        options
            .SetAuthorizationEndpointUris("/oauth2/authorize")
            .SetLogoutEndpointUris("/oauth2/logout")
            .SetTokenEndpointUris("/oauth2/token")
            .SetUserinfoEndpointUris("/oauth2/userinfo");

        options.SetIssuer(oidcConfiguration.Issuer);

        options.RegisterScopes(
            Scopes.Email,
            Scopes.Profile,
            CustomScopes.SocialWorkerRecord,
            Scopes.Roles,
            CustomScopes.Organisation
        );

        options.AllowAuthorizationCodeFlow();

        options.SetAccessTokenLifetime(TimeSpan.FromHours(1));

        if (builder.Environment.IsProduction())
        {
            var encryptionKeysConfig =
                builder.Configuration.GetSection("EncryptionKeys").Get<string[]>() ?? [];
            var signingKeysConfig =
                builder.Configuration.GetSection("SigningKeys").Get<string[]>() ?? [];

            foreach (var value in encryptionKeysConfig)
            {
                options.AddEncryptionKey(LoadKey(value));
            }

            foreach (var value in signingKeysConfig)
            {
                options.AddSigningKey(LoadKey(value));
            }

            static SecurityKey LoadKey(string configurationValue)
            {
                using var rsa = RSA.Create();
                rsa.FromXmlString(configurationValue);
                return new RsaSecurityKey(rsa.ExportParameters(includePrivateParameters: true));
            }
        }
        else
        {
            options.AddDevelopmentEncryptionCertificate().AddDevelopmentSigningCertificate();
        }

        options
            .UseAspNetCore()
            .EnableAuthorizationEndpointPassthrough()
            .EnableLogoutEndpointPassthrough()
            .EnableTokenEndpointPassthrough()
            .EnableUserinfoEndpointPassthrough()
            .EnableStatusCodePagesIntegration();
    })
    .AddValidation(options =>
    {
        // Import the configuration from the local OpenIddict server instance.
        options.UseLocalServer();
        // Register the ASP.NET Core host.
        options.UseAspNetCore();
    });

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
            Description = "API for managing accounts in the system",
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
        Description = "Enter JWT Bearer token **_only_**",
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
                    Id = "Bearer",
                },
            },
            []
        },
    };
    swaggerGenOptions.AddSecurityRequirement(securityRequirement);
});
builder
    .Services.AddOptions<SwaggerUIOptions>()
    .Configure<IHttpContextAccessor>(
        (swaggerUiOptions, httpContextAccessor) =>
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

if (!builder.Environment.IsUnitTests() && !builder.Environment.IsEndToEndTests())
{
    builder.Services.AddDbContext<IdDbContext>(options =>
        options
            .UseNpgsql(builder.Configuration.GetRequiredConnectionString("DefaultConnection"))
            .UseOpenIddict<Guid>()
    );
}

builder.AddBlobStorage();

builder
    .Services.AddEcfBaseServices()
    .AddTransient<AuthorizeAccessLinkGenerator, RoutingAuthorizeAccessLinkGenerator>()
    .AddTransient<FormFlowJourneySignInHandler>()
    .AddTransient<MatchToEcfAccountAuthenticationHandler>()
    .AddHttpContextAccessor()
    .AddSingleton<IStartupFilter, FormFlowSessionMiddlewareStartupFilter>()
    .AddFormFlow(options =>
    {
        options.JourneyRegistry.RegisterJourney(SignInJourneyState.JourneyDescriptor);
    })
    .AddSingleton<ICurrentUserIdProvider, FormFlowSessionCurrentUserIdProvider>()
    .AddTransient<SignInJourneyHelper>()
    .AddSingleton<ITagHelperInitializer<FormTagHelper>, FormTagHelperInitializer>()
    .AddFileService()
    .AddPersonMatching()
    .AddHostedService<OidcApplicationSeeder>()
    .AddScoped<IAccountsService, AccountsService>()
    .AddScoped<IOneLoginAccountLinkingService, OneLoginAccountLinkingService>();
;

builder
    .Services.AddOptions<AuthorizeAccessOptions>()
    .Bind(builder.Configuration)
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.AddTestApp();

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseWhen(
    context => !context.Request.Path.StartsWithSegments("/oauth2"),
    a =>
    {
        if (app.Environment.IsDevelopment())
        {
            a.UseDeveloperExceptionPage();
            a.UseMigrationsEndPoint();
        }
        else if (!app.Environment.IsUnitTests())
        {
            a.UseExceptionHandler("/error");
            a.UseStatusCodePagesWithReExecute("/error", "?code={0}");
        }
    }
);

app.UseCsp(csp =>
{
    var pageTemplateHelper = app.Services.GetRequiredService<PageTemplateHelper>();

    csp.ByDefaultAllow.FromSelf();

    csp.AllowScripts.FromSelf().From(pageTemplateHelper.GetCspScriptHashes()).AddNonce();

    csp.AllowStyles.FromSelf().AddNonce();

    // Ensure ASP.NET Core's auto refresh works
    // See https://github.com/dotnet/aspnetcore/issues/33068
    if (builder.Environment.IsDevelopment())
    {
        csp.AllowConnections.ToAnywhere();
    }
});

app.UseMiddleware<AppendSecurityResponseHeadersMiddleware>();

app.UseStaticFiles();

if (builder.Environment.IsProduction())
{
    app.UseDfeAnalytics();
}

if (builder.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

if (builder.Configuration["RootRedirect"] is { } rootRedirect)
{
    app.MapGet(
        "/",
        ctx =>
        {
            ctx.Response.Redirect(rootRedirect);
            return Task.CompletedTask;
        }
    );
}

app.Run();

[PublicAPI]
public partial class Program { }
