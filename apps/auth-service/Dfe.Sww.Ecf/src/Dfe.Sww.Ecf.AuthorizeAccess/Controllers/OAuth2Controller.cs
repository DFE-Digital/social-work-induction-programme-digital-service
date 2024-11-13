using System.Security.Claims;
using Dfe.Sww.Ecf.AuthorizeAccess.Infrastructure.Security;
using Dfe.Sww.Ecf.Core.DataStore.Postgres;
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
        var request =
            HttpContext.GetOpenIddictServerRequest()
            ?? throw new InvalidOperationException(
                "The OpenID Connect request cannot be retrieved."
            );

        if (!request.HasScope(CustomScopes.SocialWorkerRecord))
        {
            return Forbid(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties(
                    new Dictionary<string, string?>()
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] =
                            Errors.InvalidRequest,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            $"Requests must include the {CustomScopes.SocialWorkerRecord} scope.",
                    }
                )
            );
        }

        var application =
            await applicationManager.FindByClientIdAsync(request.ClientId ?? "")
            ?? throw new InvalidOperationException("The application cannot be found.");

        var authenticateResult = await HttpContext.AuthenticateAsync(
            AuthenticationSchemes.MatchToEcfAccount
        );

        if (!authenticateResult.Succeeded)
        {
            var parameters = Request.HasFormContentType
                ? Request.Form.ToList()
                : Request.Query.ToList();

            var serviceUrl = new Uri(request.RedirectUri!).GetLeftPart(UriPartial.Authority);
            var linkingToken = parameters
                .GroupBy(kvp => kvp.Key)
                .FirstOrDefault(kvp =>
                    kvp.Key
                    == MatchToEcfAccountAuthenticationHandler
                        .AuthenticationPropertiesItemKeys
                        .LinkingToken
                )
                ?.Select(kvp => kvp.Value)
                .FirstOrDefault();

            var authenticationProperties = new AuthenticationProperties()
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
                        await applicationManager.GetDisplayNameAsync(application)
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

        var user = authenticateResult.Principal;
        var subject =
            user.FindFirstValue(ClaimTypes.Subject)
            ?? throw new InvalidOperationException(
                $"Principal does not contain a '{ClaimTypes.Subject}' claim."
            );

        var identity = new ClaimsIdentity(
            claims: user.Claims,
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: System.Security.Claims.ClaimTypes.Name,
            roleType: null
        );

        identity
            .SetClaim(ClaimTypes.Subject, subject)
            .SetClaim(ClaimTypes.Email, user.FindFirstValue(ClaimTypes.Email) ?? "");

        var oneLoginUser = await dbContext
            .OneLoginUsers.Include(o => o.Person)
            .SingleAsync(u => u.Subject == subject);

        if (oneLoginUser.Person is { } person)
        {
            identity
                .SetClaim(ClaimTypes.Trn, person.Trn ?? "")
                .SetClaim(
                    System.Security.Claims.ClaimTypes.Name,
                    $"{person.FirstName} {person.LastName}"
                );
        }

        identity.SetScopes(request.GetScopes());
        identity.SetResources(
            await scopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync()
        );
        identity.SetDestinations(GetDestinations);

        return SignIn(
            new ClaimsPrincipal(identity),
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
        );
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

        if (request.IdTokenHint is null)
        {
            return BadRequest();
        }

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
            case ClaimTypes.Trn:
                yield return Destinations.AccessToken;
                yield return Destinations.IdentityToken;
                yield break;

            case ClaimTypes.Email:
                if (claim.Subject!.HasScope(Scopes.Email))
                {
                    yield return Destinations.IdentityToken;
                }

                yield break;

            case ClaimTypes.OneLoginIdToken:
                yield return Destinations.IdentityToken;
                yield break;

            default:
                yield break;
        }
    }
}
