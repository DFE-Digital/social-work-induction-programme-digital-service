using System.Diagnostics.CodeAnalysis;
using Dfe.Sww.Ecf.Frontend.Configuration;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using OpenIddict.Abstractions;

namespace Dfe.Sww.Ecf.Frontend.Installers;

[ExcludeFromCodeCoverage]
public static class InstallAuthentication
{
    public static void AddEcfAuthentication(
        this IServiceCollection services,
        IConfigurationSection configurationSection
    )
    {
        services
            .AddOptions<OidcConfiguration>()
            .Bind(configurationSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var ecfAuthenticationOptions = configurationSection.Get<OidcConfiguration>();
        if (ecfAuthenticationOptions is null)
        {
            throw new InvalidOperationException(
                "Unable to parse configuration for ECF authentication."
            );
        }

        services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                if (ecfAuthenticationOptions.CookieName is not null)
                {
                    options.Cookie.Name = ecfAuthenticationOptions.CookieName;
                }
                options.ExpireTimeSpan = TimeSpan.FromMinutes(
                    ecfAuthenticationOptions.SessionLifetimeMinutes
                );
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.SlidingExpiration = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            })
            .AddOpenIdConnect(options =>
            {
                options.Authority = ecfAuthenticationOptions.AuthorityUrl;
                options.ClientId = ecfAuthenticationOptions.ClientId;
                options.ClientSecret = ecfAuthenticationOptions.ClientSecret;

                options.ResponseMode = OpenIddictConstants.ResponseModes.Query;
                options.ResponseType = OpenIddictConstants.ResponseTypes.Code;
                options.NonceCookie.SameSite = SameSiteMode.Lax;
                options.CorrelationCookie.SameSite = SameSiteMode.Lax;
                options.CallbackPath = new PathString(ecfAuthenticationOptions.CallbackUrl);
                options.SignedOutCallbackPath = new PathString(
                    ecfAuthenticationOptions.SignedOutCallbackUrl
                );
                options.UsePkce = true;

                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.AddRange(ecfAuthenticationOptions.Scopes);

                options.GetClaimsFromUserInfoEndpoint = true;
                options.SaveTokens = true;

                options.Events.OnRedirectToIdentityProvider = context =>
                {
                    if (
                        context.HttpContext.Request.Query.TryGetValue(
                            "LinkingToken",
                            out var linkingToken
                        )
                    )
                    {
                        context.ProtocolMessage.SetParameter("LinkingToken", linkingToken);
                    }

                    return Task.CompletedTask;
                };
            });
    }
}
