using System.Security.Claims;
using Dfe.Sww.Ecf.AuthorizeAccess.Infrastructure.Security;
using Dfe.Sww.Ecf.Core.DataStore.Postgres;
using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Controllers;

public class OAuth2Controller(
    EcfDbContext dbContext,
    IOpenIddictApplicationManager applicationManager,
    IOpenIddictScopeManager scopeManager
) : Controller
{
    [HttpGet("~/oauth2/authorize")]
    [HttpPost("~/oauth2/authorize"), Produces("application/json")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Authorize()
    {
        var request = HttpContext.GetOpenIddictServerRequest();
        if (request is null)
        {
            throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");
        }

        var application =
            await applicationManager.FindByClientIdAsync(request.ClientId ?? "")
            ?? throw new InvalidOperationException("The application cannot be found.");

        var authenticateResult = await HttpContext.AuthenticateAsync(
            AuthenticationSchemes.MatchToEcfAccount
        );

        if (!authenticateResult.Succeeded || authenticateResult.Principal == null)
        {
            return await HandleAuthenticationFailureAsync(request, application);
        }

        var principal = authenticateResult.Principal;
        var subject =
            principal.FindFirstValue(ClaimTypes.Subject)
            ?? throw new InvalidOperationException(
                $"Principal does not contain a '{ClaimTypes.Subject}' claim."
            );

        var identity = CreateIdentity(principal, subject);

        var oneLoginUser = await dbContext
            .OneLoginUsers.Include(o => o.Person)
            .SingleAsync(u => u.Subject == subject);

        await BuildClaimsAsync(identity, request, oneLoginUser);

        await SetIdentityProperties(identity, request);

        return SignIn(
            new ClaimsPrincipal(identity),
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
        );
    }

    private static ClaimsIdentity CreateIdentity(ClaimsPrincipal userPrincipal, string subject)
    {
        var identity = new ClaimsIdentity(
            claims: userPrincipal.Claims,
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: System.Security.Claims.ClaimTypes.Name,
            roleType: System.Security.Claims.ClaimTypes.Role
        );

        identity.SetClaim(ClaimTypes.Subject, subject);
        return identity;
    }

    private async Task SetIdentityProperties(ClaimsIdentity identity, OpenIddictRequest request)
    {
        identity.SetScopes(request.GetScopes());
        identity.SetResources(
            await scopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync()
        );
        identity.SetDestinations(GetDestinations);
    }

    private async Task BuildClaimsAsync(
        ClaimsIdentity identity,
        OpenIddictRequest request,
        OneLoginUser oneLoginUser
    )
    {
        if (oneLoginUser.Person is null)
        {
            return;
        }

        var claimsBuilder = new ClaimsBuilder(identity, request);

        await claimsBuilder
            .AddIfScope(
                Scopes.Profile,
                System.Security.Claims.ClaimTypes.Name,
                () => oneLoginUser.Person.FirstName + " " + oneLoginUser.Person.LastName
            )
            .AddIfScope(Scopes.Email, ClaimTypes.Email, () => oneLoginUser.Person.EmailAddress)
            .AddIfScope(
                CustomScopes.SocialWorkerRecord,
                ClaimTypes.Trn,
                () => oneLoginUser.Person.Trn
            )
            .AddIfScope(CustomScopes.Person, ClaimTypes.PersonId, () => oneLoginUser.PersonId.ToString())
            .AddRoleClaimsIfScopeAsync(Scopes.Roles, oneLoginUser.Person.PersonId, dbContext);

        await claimsBuilder.AddOrganisationIdClaimIfScopeAsync(CustomScopes.Organisation, oneLoginUser.Person.PersonId,
            dbContext);

        // Claim only false for ECSWs that are pending registration
        if (oneLoginUser.Person.Status == PersonStatus.PendingRegistration)
        {
            claimsBuilder
                .AddIfScope(
                    CustomScopes.EcswRegistered,
                    ClaimTypes.IsEcswRegistered,
                    () => "false"
                );
        }
    }

    private async Task<IActionResult> HandleAuthenticationFailureAsync(
        OpenIddictRequest request,
        object application
    )
    {
        var parameters = Request.HasFormContentType
            ? Request.Form.ToList()
            : Request.Query.ToList();

        var serviceUrl = GetRedirectUri(request);
        var serviceName = await applicationManager.GetDisplayNameAsync(application);
        var linkingToken = parameters
            .FirstOrDefault(kvp =>
                kvp.Key
                == MatchToEcfAccountAuthenticationHandler
                    .AuthenticationPropertiesItemKeys
                    .LinkingToken
            )
            .Value;

        var authenticationProperties = new AuthenticationProperties
        {
            RedirectUri = Request.PathBase + Request.Path + QueryString.Create(parameters),
            Items =
            {
                {
                    MatchToEcfAccountAuthenticationHandler
                        .AuthenticationPropertiesItemKeys
                        .OneLoginAuthenticationScheme,
                    "OneLogin"
                },
                {
                    MatchToEcfAccountAuthenticationHandler
                        .AuthenticationPropertiesItemKeys
                        .ServiceName,
                    serviceName
                },
                {
                    MatchToEcfAccountAuthenticationHandler
                        .AuthenticationPropertiesItemKeys
                        .ServiceUrl,
                    serviceUrl
                },
                {
                    MatchToEcfAccountAuthenticationHandler
                        .AuthenticationPropertiesItemKeys
                        .LinkingToken,
                    linkingToken
                },
            },
        };

        return Challenge(authenticationProperties, AuthenticationSchemes.MatchToEcfAccount);
    }

    private static string GetRedirectUri(OpenIddictRequest request)
    {
        if (string.IsNullOrEmpty(request.RedirectUri))
        {
            throw new InvalidOperationException("RedirectUri is missing or invalid.");
        }

        return new Uri(request.RedirectUri).GetLeftPart(UriPartial.Authority);
    }

    [HttpPost("~/oauth2/token")]
    [IgnoreAntiforgeryToken]
    [Produces("application/json")]
    public async Task<IActionResult> Token()
    {
        var request =
            HttpContext.GetOpenIddictServerRequest()
            ?? throw new InvalidOperationException(
                "The OpenID Connect request cannot be retrieved."
            );

        if (!request.IsAuthorizationCodeGrantType() && !request.IsRefreshTokenGrantType())
        {
            throw new InvalidOperationException("The specified grant type is not supported.");
        }

        var result = await HttpContext.AuthenticateAsync(
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
        );

        var identity = new ClaimsIdentity(
            result.Principal!.Claims,
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: System.Security.Claims.ClaimTypes.Name,
            roleType: System.Security.Claims.ClaimTypes.Role
        );

        identity.SetDestinations(GetDestinations);

        return SignIn(
            new ClaimsPrincipal(identity),
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
        );
    }

    [HttpGet("~/oauth2/logout")]
    public async Task<IActionResult> Logout()
    {
        // Although the spec allows for logout requests without an id_token_hint, we require one so we can
        // a) extract the One Login ID token and;
        // b) know which authentication scheme to sign out with.

        var request =
            HttpContext.GetOpenIddictServerRequest()
            ?? throw new InvalidOperationException(
                "The OpenID Connect request cannot be retrieved."
            );

        var authenticateResult = await HttpContext.AuthenticateAsync(
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
        );

        // We need to sign out with One Login and then complete the OIDC sign out request.
        // We do it by calling SignOutAsync with OpenIddict first, capturing the Location header from its redirect
        // then redirecting to OneLogin with that URL as the RedirectUri.

        await HttpContext.SignOutAsync(
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
            new AuthenticationProperties() { RedirectUri = "/" }
        );

        var authenticationProperties = new AuthenticationProperties()
        {
            RedirectUri = HttpContext.Response.Headers.Location,
        };
        var oneLoginIdToken = authenticateResult.Principal!.FindFirstValue(
            ClaimTypes.OneLoginIdToken
        )!;
        authenticationProperties.SetParameter(OpenIdConnectParameterNames.IdToken, oneLoginIdToken);

        return SignOut(authenticationProperties, "OneLogin");
    }

    [Authorize(AuthenticationSchemes = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)]
    [HttpGet("~/oauth2/userinfo")]
    [HttpPost("~/oauth2/userinfo")]
    [Produces("application/json")]
    public async Task<IActionResult> UserInfo()
    {
        var subject = User.GetClaim(ClaimTypes.Subject)!;

        var claims = new Dictionary<string, object>(StringComparer.Ordinal)
        {
            [ClaimTypes.Subject] = subject,
        };

        var oneLoginUser = await dbContext
            .OneLoginUsers.Include(o => o.Person)
            .SingleAsync(u => u.Subject == subject);

        if (oneLoginUser.Person is { } person)
        {
            claims.Add(ClaimTypes.Trn, person.Trn ?? "");
            claims.Add(
                System.Security.Claims.ClaimTypes.Name,
                $"{person.FirstName} {person.LastName}"
            );
        }

        if (User.HasScope(Scopes.Email))
        {
            claims.Add(ClaimTypes.Email, oneLoginUser.Email);
        }

        if (oneLoginUser.VerificationRoute != OneLoginUserVerificationRoute.OneLogin)
        {
            return Ok(claims);
        }

        claims.Add(ClaimTypes.OneLoginVerifiedNames, oneLoginUser.VerifiedNames!);
        claims.Add(ClaimTypes.OneLoginVerifiedBirthDates, oneLoginUser.VerifiedDatesOfBirth!);

        return Ok(claims);
    }

    private static IEnumerable<string> GetDestinations(Claim claim)
    {
        switch (claim.Type)
        {
            case ClaimTypes.Subject:
            case System.Security.Claims.ClaimTypes.Name:
                yield return Destinations.AccessToken;
                yield return Destinations.IdentityToken;
                yield break;
            case ClaimTypes.Trn:
                if (claim.Subject!.HasScope(CustomScopes.SocialWorkerRecord))
                {
                    yield return Destinations.AccessToken;
                    yield return Destinations.IdentityToken;
                }
                yield break;

            case ClaimTypes.Email:
                if (claim.Subject!.HasScope(Scopes.Email))
                {
                    yield return Destinations.IdentityToken;
                }

                yield break;

            case System.Security.Claims.ClaimTypes.Role:
                if (claim.Subject!.HasScope(Scopes.Roles))
                {
                    yield return Destinations.AccessToken;
                    yield return Destinations.IdentityToken;
                }
                yield break;

            case ClaimTypes.OneLoginIdToken:
                yield return Destinations.IdentityToken;
                yield break;

            case ClaimTypes.OrganisationId:
                if (claim.Subject!.HasScope(CustomScopes.Organisation))
                {
                    yield return Destinations.AccessToken;
                    yield return Destinations.IdentityToken;
                }
                yield break;
            case ClaimTypes.IsEcswRegistered:
                if (claim.Subject!.HasScope(CustomScopes.EcswRegistered))
                {
                    yield return Destinations.AccessToken;
                    yield return Destinations.IdentityToken;
                }
                yield break;
            case ClaimTypes.PersonId:
                if (claim.Subject!.HasScope(CustomScopes.Person))
                {
                    yield return Destinations.AccessToken;
                    yield return Destinations.IdentityToken;
                }
                yield break;

            default:
                yield break;
        }
    }
}
