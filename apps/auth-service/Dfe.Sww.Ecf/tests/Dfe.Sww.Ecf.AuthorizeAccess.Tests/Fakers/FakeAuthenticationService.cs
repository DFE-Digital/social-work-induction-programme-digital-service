using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Tests.Fakers;

internal sealed class FakeAuthenticationService : IAuthenticationService
{
    private readonly ClaimsPrincipal _principal;

    public FakeAuthenticationService(string subject)
    {
        var identity = new ClaimsIdentity(
            [
                new Claim(System.Security.Claims.ClaimTypes.NameIdentifier, subject),
                new Claim(System.Security.Claims.ClaimTypes.Name, subject),
                new Claim(ClaimTypes.Subject, subject)
            ],
            authenticationType: "TestAuth");
        _principal = new ClaimsPrincipal(identity);
    }

    public Task<AuthenticateResult> AuthenticateAsync(HttpContext context, string? scheme)
    {
        if (scheme == Infrastructure.Security.AuthenticationSchemes.MatchToEcfAccount)
        {
            return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(_principal, scheme)));
        }
        return Task.FromResult(AuthenticateResult.NoResult());
    }

    public Task ChallengeAsync(HttpContext context, string? scheme, AuthenticationProperties? properties) =>
        Task.CompletedTask;

    public Task ForbidAsync(HttpContext context, string? scheme, AuthenticationProperties? properties) =>
        Task.CompletedTask;

    public Task SignInAsync(HttpContext context, string? scheme, ClaimsPrincipal principal, AuthenticationProperties? properties) =>
        Task.CompletedTask;

    public Task SignOutAsync(HttpContext context, string? scheme, AuthenticationProperties? properties) =>
        Task.CompletedTask;
}
