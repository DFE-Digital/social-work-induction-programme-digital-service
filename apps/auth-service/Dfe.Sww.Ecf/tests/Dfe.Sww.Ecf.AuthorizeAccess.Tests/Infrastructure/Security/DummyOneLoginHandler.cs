using Microsoft.AspNetCore.Authentication;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Tests.Infrastructure.Security;

public class DummyOneLoginHandler : IAuthenticationHandler
{
    public Task<AuthenticateResult> AuthenticateAsync() => Task.FromResult(AuthenticateResult.NoResult());

    public Task ChallengeAsync(AuthenticationProperties? properties) => throw new NotSupportedException();

    public Task ForbidAsync(AuthenticationProperties? properties) => throw new NotSupportedException();

    public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context) => Task.CompletedTask;
}
