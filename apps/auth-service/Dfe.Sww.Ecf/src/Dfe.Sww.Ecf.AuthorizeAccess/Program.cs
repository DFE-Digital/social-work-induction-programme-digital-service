using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
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
using Dfe.Sww.Ecf.Core.DataStore.Postgres;
using Dfe.Sww.Ecf.Core.Services.Accounts;
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
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using static OpenIddict.Abstractions.OpenIddictConstants;
using OpenIddict.Server;
using OpenIddict.Server.AspNetCore;
using Microsoft.IdentityModel.Tokens.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AppInfo>(
  builder.Configuration.GetRequiredSection("AppInfo")
);

var featureFlags = builder.Configuration
    .GetRequiredSection("FeatureFlags")
    .Get<FeatureFlags>()!;

var oneLoginSection = builder.Configuration.GetRequiredSection("OneLogin");
builder.Services.Configure<OneLoginConfiguration>(oneLoginSection);
var oneLoginConfig = oneLoginSection.Get<OneLoginConfiguration>()!;

builder.Services
    .AddSingleton(oneLoginConfig)
    .AddSingleton(featureFlags);

SecretClient? secretClient = null;
CertificateClient? certificateClient = null;
if (featureFlags.EnableOneLoginCertificateRotation || featureFlags.EnableOpenIdCertificates)
{
    var keyVaultUri = new Uri(builder.Configuration.GetRequiredValue("KeyVaultUri"));
    secretClient = new SecretClient(keyVaultUri, new DefaultAzureCredential());
    certificateClient = new CertificateClient(keyVaultUri, new DefaultAzureCredential());
    builder.Services
        .AddSingleton(_ => secretClient)
        .AddSingleton(_ => certificateClient);
}

builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

// Add environment variables settings integration for Azure
builder.Configuration.AddEnvironmentVariables();
builder.AddServiceDefaults();

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

        options.MetadataAddress = oneLoginConfig.Url + "/.well-known/openid-configuration";
        options.ClientAssertionJwtAudience = oneLoginConfig.Url + "/token";

        if (featureFlags.EnableOneLoginCertificateRotation && secretClient is not null)
        {
            KeyVaultSecret pfxSecret = secretClient
                .GetSecret(oneLoginConfig.CertificateName);
            var pfxBytes = Convert.FromBase64String(pfxSecret.Value);
            var certificate = new X509Certificate2(
                pfxBytes,
                (string?)null,
                X509KeyStorageFlags.EphemeralKeySet
            );
            var rsaPrivateKey = certificate.GetRSAPrivateKey();
            options.ClientAuthenticationCredentials = new SigningCredentials(
                new RsaSecurityKey(rsaPrivateKey!),
                SecurityAlgorithms.RsaSha256
            );
        }
        else
        {
            using var rsa = RSA.Create();
            rsa.ImportFromPem(oneLoginConfig.PrivateKeyPem);

            options.ClientAuthenticationCredentials = new SigningCredentials(
                new RsaSecurityKey(rsa.ExportParameters(includePrivateParameters: true)),
                SecurityAlgorithms.RsaSha256
            );
        }

        options.ClientId =
            oneLoginConfig.ClientId
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

            journeyInstanceId = null;
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
            CustomScopes.Organisation,
            CustomScopes.EcswRegistered,
            CustomScopes.Person
        );

        options.AllowAuthorizationCodeFlow();

        options.SetAccessTokenLifetime(TimeSpan.FromHours(1));

        options
            .UseAspNetCore()
            .EnableAuthorizationEndpointPassthrough()
            .EnableLogoutEndpointPassthrough()
            .EnableTokenEndpointPassthrough()
            .EnableUserinfoEndpointPassthrough()
            .EnableStatusCodePagesIntegration();

        if (featureFlags.EnableOpenIdCertificates && certificateClient is not null)
        {
            var certName = builder.Configuration.GetRequiredValue("Oidc:SigningCertificateName");
            var signingCert = certificateClient
                .GetX509CertificateAsync(certName)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
            //options.AddSigningCertificate(signingCert);
            // --- Create a custom JsonWebKey for SIGNING ---
            // Extract RSA public key parameters
            var rsaSigningPublicKey = signingCert.GetRSAPublicKey() ??
                                    throw new InvalidOperationException("Signing certificate is not RSA.");
            var signingParameters = rsaSigningPublicKey.ExportParameters(false); // Export public parameters

            // Onelogin gives this error: "Failed to fetch or parse JWKS to verify signature of private_key_jwt"
            // if anything other than kty, e, use, kid and n fields are present in the key.
            var signingJwk = new JsonWebKey
            {
                Kid = signingCert.Thumbprint, // Use thumbprint as Kid
                Kty = JsonWebAlgorithmsKeyTypes.RSA,
                Use = JsonWebKeyUseNames.Sig,
                N = Base64UrlEncoder.Encode(signingParameters.Modulus),
                E = Base64UrlEncoder.Encode(signingParameters.Exponent)
                // IMPORTANT: DO NOT set Alg, X5t, X5c, KeyOps, Oth here.
                // JsonWebKey by default will not serialize null properties.
            };

            // Add the signing certificate (private key) for signing operations
            // AND specify the *public* JWK to be used in the JWKS endpoint.
            options.AddSigningKey(signingJwk);
                
            certName = builder.Configuration.GetRequiredValue("Oidc:EncryptionCertificateName");
            var encryptionCert = certificateClient
                .GetX509CertificateAsync(certName)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
            options.AddEncryptionCertificate(encryptionCert);
        }

        if (featureFlags.EnableDevelopmentOpenIdCertificates)
        {
            options.AddDevelopmentEncryptionCertificate().AddDevelopmentSigningCertificate();
        }
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
    .AddFormFlow(options =>
    {
        options.JourneyRegistry.RegisterJourney(SignInJourneyState.JourneyDescriptor);
    })
    .AddSingleton<ICurrentUserIdProvider, FormFlowSessionCurrentUserIdProvider>()
    .AddTransient<SignInJourneyHelper>()
    .AddSingleton<ITagHelperInitializer<FormTagHelper>, FormTagHelperInitializer>()
    .AddHostedService<OidcApplicationSeeder>()
    .AddScoped<IAccountsService, AccountsService>()
    .AddScoped<IOneLoginAccountLinkingService, OneLoginAccountLinkingService>()
    .AddSingleton(sp => sp.GetRequiredService<IOptions<AppInfo>>().Value)
    .AddSingleton(sp => sp.GetRequiredService<IOptions<OneLoginConfiguration>>().Value);

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
