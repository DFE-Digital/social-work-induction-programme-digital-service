using System.Diagnostics.CodeAnalysis;
using Dfe.Sww.Ecf.AuthorizeAccess.Infrastructure.FormFlow;
using Microsoft.AspNetCore.Authentication;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Infrastructure.Security;

public class MatchToEcfAccountAuthenticationHandler(SignInJourneyHelper helper)
    : IAuthenticationHandler
{
    private AuthenticationScheme? _scheme;
    private HttpContext? _context;

    public async Task<AuthenticateResult> AuthenticateAsync()
    {
        EnsureInitialized();

        var journeyInstance = await helper.UserInstanceStateProvider.GetSignInJourneyInstanceAsync(
            _context
        );

        if (journeyInstance is null)
        {
            return AuthenticateResult.NoResult();
        }

        var ticket = journeyInstance.State.AuthenticationTicket;

        return ticket is null ? AuthenticateResult.NoResult() : AuthenticateResult.Success(ticket);
    }

    public async Task ChallengeAsync(AuthenticationProperties? properties)
    {
        EnsureInitialized();

        if (
            properties is null
            || !TryGetNonNullItem(AuthenticationPropertiesItemKeys.ServiceName, out var serviceName)
            || !TryGetNonNullItem(AuthenticationPropertiesItemKeys.ServiceUrl, out var serviceUrl)
        )
        {
            throw new InvalidOperationException(
                $"{nameof(AuthenticationProperties)} is missing one or more items."
            );
        }

        properties.Items.TryGetValue(
            AuthenticationPropertiesItemKeys.LinkingToken,
            out var linkingToken
        );

        var journeyInstance =
            await helper.UserInstanceStateProvider.GetOrCreateSignInJourneyInstanceAsync(
                _context,
                createState: () =>
                    new SignInJourneyState(
                        properties.RedirectUri ?? "/",
                        serviceName,
                        serviceUrl,
                        linkingToken
                    ),
                updateState: state => state.Reset()
            );

        var result = helper.SignInWithOneLogin(journeyInstance);
        await result.ExecuteAsync(_context);
        return;

        bool TryGetNonNullItem(string key, [NotNullWhen(true)] out string? value)
        {
            value = default;
            return properties.Items.TryGetValue(key, out value) && value is not null;
        }
    }

    public Task ForbidAsync(AuthenticationProperties? properties)
    {
        throw new NotSupportedException();
    }

    public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
    {
        _scheme = scheme;
        _context = context;
        return Task.CompletedTask;
    }

    [MemberNotNull(nameof(_context), nameof(_scheme))]
    private void EnsureInitialized()
    {
        if (_context is null || _scheme is null)
        {
            throw new InvalidOperationException("Not initialized.");
        }
    }

    public static class AuthenticationPropertiesItemKeys
    {
        public const string OneLoginAuthenticationScheme = "OneLoginAuthenticationScheme";
        public const string ServiceName = "ServiceName";
        public const string ServiceUrl = "ServiceUrl";
        public const string LinkingToken = "LinkingToken";
    }
}
