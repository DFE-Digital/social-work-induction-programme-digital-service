using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using Dfe.Sww.Ecf.AuthorizeAccess.Infrastructure.FormFlow;
using Dfe.Sww.Ecf.AuthorizeAccess.Infrastructure.Security;
using Dfe.Sww.Ecf.AuthorizeAccess.Infrastructure.Security.Configuration;
using Dfe.Sww.Ecf.Core.DataStore.Postgres;
using Dfe.Sww.Ecf.UiCommon.FormFlow;
using GovUk.OneLogin.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OneLoginOptions = Dfe.Sww.Ecf.AuthorizeAccess.Infrastructure.Security.Configuration.OneLoginOptions;

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
            .GetRequiredService<IOptions<FeatureFlagOptions>>()
            .Value;

        if (featureFlags.EnableMigrationsEndpoint)
        {
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        }

        if (featureFlags.EnableForwardedHeaders)
        {
            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.All;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });
        }

        if (featureFlags.EnableHttpStrictTransportSecurity)
        {
            builder.Services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(365);
            });
        }

        if (featureFlags.EnableMsDotNetDataProtectionServices)
        {
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
            .GetRequiredService<IOptions<FeatureFlagOptions>>()
            .Value;

        if (featureFlags.EnableForwardedHeaders)
        {
            app.UseForwardedHeaders();
        }

        if (featureFlags.EnableHttpStrictTransportSecurity)
        {
            app.UseHsts();
        }

        app.MapGet("/health", async context => { await context.Response.WriteAsync("OK"); });

        app.UseHealthChecks("/status");

        return app;
    }

    public static IHostApplicationBuilder AddAuthentication(
        this IHostApplicationBuilder builder, ILogger logger)
    {
        using var scope = logger.BeginScope("Adding authentication");

        var oneLoginSection = builder.Configuration.GetRequiredSection(OneLoginOptions.ConfigurationKey);
        builder.Services.AddOptions<OneLoginOptions>().Bind(oneLoginSection);

        var serviceProvider = builder.Services.BuildServiceProvider();
        var oneLoginConfig = serviceProvider.GetRequiredService<IOptions<OneLoginOptions>>().Value;
        var featureFlags = serviceProvider.GetRequiredService<IOptions<FeatureFlagOptions>>().Value;
        var certificateClient = serviceProvider.GetService<CertificateClient>();

        builder
            .Services.AddAuthentication(options =>
            {
                logger.LogInformation("Registering authentication schemes");

                options.DefaultAuthenticateScheme = AuthenticationSchemes.MatchToEcfAccount;
                options.DefaultChallengeScheme = AuthenticationSchemes.MatchToEcfAccount;

                options.AddScheme(
                    AuthenticationSchemes.FormFlowJourney,
                    scheme => { scheme.HandlerType = typeof(FormFlowJourneySignInHandler); }
                );

                options.AddScheme(
                    AuthenticationSchemes.MatchToEcfAccount,
                    scheme => { scheme.HandlerType = typeof(MatchToEcfAccountAuthenticationHandler); }
                );
            })
            .AddOneLogin(options =>
            {
                logger.LogInformation("Registering OneLogin authentication provider");
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

                if (featureFlags.EnableOneLoginCertificateRotation && certificateClient is not null)
                {
                    logger.LogInformation("Using certificate client to get certificate for OneLogin");
                    var signingCert = certificateClient
                        .GetX509CertificateAsync(oneLoginConfig.CertificateName!)
                        .ConfigureAwait(false)
                        .GetAwaiter()
                        .GetResult();
                    var rsaPrivateKey = signingCert.GetRSAPrivateKey();
                    if (rsaPrivateKey is null)
                    {
                        throw new InvalidOperationException("Unable to get private key from certificate for OneLogin");
                    }

                    options.ClientAuthenticationCredentials = new SigningCredentials(
                        new RsaSecurityKey(rsaPrivateKey) { KeyId = signingCert.Thumbprint },
                        SecurityAlgorithms.RsaSha256
                    );
                }
                else
                {
                    logger.LogInformation("Using private key from PrivateKeyPem for OneLogin");
                    using var rsa = RSA.Create();
                    rsa.ImportFromPem(oneLoginConfig.PrivateKeyPem);

                    options.ClientAuthenticationCredentials = new SigningCredentials(
                        new RsaSecurityKey(rsa.ExportParameters(true)),
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
        return builder;
    }

    public static IHostApplicationBuilder AddOpenIddict(
        this IHostApplicationBuilder builder, ILogger logger)
    {
        using var scope = logger.BeginScope("Adding OpenIdDict");

        var serviceProvider = builder.Services.BuildServiceProvider();
        var oidcConfiguration = serviceProvider.GetRequiredService<IOptions<OidcOptions>>().Value;
        var featureFlags = serviceProvider.GetRequiredService<IOptions<FeatureFlagOptions>>().Value;
        var certificateClient = serviceProvider.GetService<CertificateClient>();

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
                    OpenIddictConstants.Scopes.Email,
                    OpenIddictConstants.Scopes.Profile,
                    CustomScopes.SocialWorkerRecord,
                    OpenIddictConstants.Scopes.Roles,
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
                    logger.LogInformation("Using certificate client to get certificates for OpenID");
                    if (oidcConfiguration.SigningCertificateName is null ||
                        oidcConfiguration.EncryptionCertificateName is null)
                    {
                        throw new InvalidConfigurationException(
                            "Missing OIDC:SigningCertificateName or OIDC:EncryptionCertificateName"
                        );
                    }

                    var signingCert = certificateClient
                        .GetX509CertificateAsync(oidcConfiguration.SigningCertificateName)
                        .ConfigureAwait(false)
                        .GetAwaiter()
                        .GetResult();
                    options.AddSigningCertificate(signingCert);

                    var encryptionCert = certificateClient
                        .GetX509CertificateAsync(oidcConfiguration.EncryptionCertificateName)
                        .ConfigureAwait(false)
                        .GetAwaiter()
                        .GetResult();
                    options.AddEncryptionCertificate(encryptionCert);
                }

                if (featureFlags.EnableDevelopmentOpenIdCertificates)
                {
                    logger.LogInformation("Using development certificates for OpenID");
                    options.AddDevelopmentEncryptionCertificate().AddDevelopmentSigningCertificate();
                }
            })
            .AddValidation(options =>
            {
                logger.LogInformation("Registering OpenIddict token validation handler");
                // Import the configuration from the local OpenIddict server instance.
                options.UseLocalServer();
                // Register the ASP.NET Core host.
                options.UseAspNetCore();
            });
        return builder;
    }
}
