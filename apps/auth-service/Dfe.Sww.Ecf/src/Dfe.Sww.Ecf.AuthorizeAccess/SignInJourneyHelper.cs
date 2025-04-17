using System.Diagnostics;
using System.Security.Claims;
using Dfe.Sww.Ecf.AuthorizeAccess.Infrastructure.Security;
using Dfe.Sww.Ecf.Core.DataStore.Postgres;
using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using Dfe.Sww.Ecf.Core.Services.Accounts;
using Dfe.Sww.Ecf.UiCommon.FormFlow;
using Dfe.Sww.Ecf.UiCommon.FormFlow.State;
using GovUk.OneLogin.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Dfe.Sww.Ecf.AuthorizeAccess;

public class SignInJourneyHelper(
    EcfDbContext dbContext,
    IOneLoginAccountLinkingService oneLoginAccountLinkingService,
    AuthorizeAccessLinkGenerator linkGenerator,
    IOptions<AuthorizeAccessOptions> optionsAccessor,
    IUserInstanceStateProvider userInstanceStateProvider,
    IClock clock
)
{
    public const string AuthenticationOnlyVtr = """["Cl.Cm"]""";
    public const string AuthenticationAndIdentityVerificationVtr = """["Cl.Cm.P2"]""";

    public AuthorizeAccessLinkGenerator LinkGenerator { get; } = linkGenerator;

    public IUserInstanceStateProvider UserInstanceStateProvider { get; } =
        userInstanceStateProvider;

    private bool ShowDebugPages => optionsAccessor.Value.ShowDebugPages;

    public IResult SignInWithOneLogin(JourneyInstance<SignInJourneyState> journeyInstance) =>
        OneLoginChallenge(journeyInstance, AuthenticationOnlyVtr);

    private IResult VerifyIdentityWithOneLogin(
        JourneyInstance<SignInJourneyState> journeyInstance
    ) => OneLoginChallenge(journeyInstance, AuthenticationAndIdentityVerificationVtr);

    public async Task<IResult> OnOneLoginCallback(
        JourneyInstance<SignInJourneyState> journeyInstance,
        AuthenticationTicket ticket
    )
    {
        if (!ticket.Properties.TryGetVectorOfTrust(out var vtr))
        {
            throw new InvalidOperationException("No vtr.");
        }

        if (vtr == AuthenticationOnlyVtr)
        {
            await OnUserAuthenticated(journeyInstance, ticket);
        }
        else
        {
            Debug.Assert(vtr == AuthenticationAndIdentityVerificationVtr);
            Debug.Assert(journeyInstance.State.OneLoginAuthenticationTicket is not null);
            await OnUserVerified(journeyInstance, ticket);
        }

        return ShowDebugPages
            ? Results.Redirect(LinkGenerator.DebugIdentity(journeyInstance.InstanceId))
            : GetNextPage(journeyInstance);
    }

    public async Task OnUserAuthenticated(
        JourneyInstance<SignInJourneyState> journeyInstance,
        AuthenticationTicket ticket
    )
    {
        var sub =
            ticket.Principal.FindFirstValue("sub")
            ?? throw new InvalidOperationException("No sub claim.");
        var email =
            ticket.Principal.FindFirstValue("email")
            ?? throw new InvalidOperationException("No email claim.");

        var oneLoginUser = await dbContext
            .OneLoginUsers.Include(u => u.Person)
            .SingleOrDefaultAsync(u => u.Subject == sub);

        if (oneLoginUser is null)
        {
            oneLoginUser = new OneLoginUser
            {
                Subject = sub,
                Email = email,
                FirstOneLoginSignIn = clock.UtcNow,
                LastOneLoginSignIn = clock.UtcNow,
            };
            dbContext.OneLoginUsers.Add(oneLoginUser);
        }
        else
        {
            oneLoginUser.LastOneLoginSignIn = clock.UtcNow;

            // Email may have changed since the last sign in - ensure we update it.
            // TODO Should we emit an event if it has changed?
            oneLoginUser.Email = email;

            if (oneLoginUser.PersonId is not null)
            {
                oneLoginUser.LastSignIn = clock.UtcNow;
            }
        }

        if (await TryApplyLinkingToken(journeyInstance) is { } result)
        {
            oneLoginUser.PersonId = result.PersonId;
            oneLoginUser.FirstSignIn = clock.UtcNow;
            oneLoginUser.LastSignIn = clock.UtcNow;
            oneLoginUser.MatchRoute = OneLoginUserMatchRoute.LinkingToken;
        }

        await dbContext.SaveChangesAsync();

        await journeyInstance.UpdateStateAsync(state =>
        {
            state.Reset();
            state.OneLoginAuthenticationTicket = ticket;

            if (oneLoginUser.VerificationRoute is not null)
            {
                Debug.Assert(oneLoginUser.VerifiedNames is not null);
                Debug.Assert(oneLoginUser.VerifiedDatesOfBirth is not null);
                state.SetVerified(oneLoginUser.VerifiedNames!, oneLoginUser.VerifiedDatesOfBirth!);
            }

            if (oneLoginUser.PersonId is not null && !ShowDebugPages)
            {
                Complete(state);
            }
        });
    }

    public Task OnUserVerified(
        JourneyInstance<SignInJourneyState> journeyInstance,
        AuthenticationTicket ticket
    )
    {
        var verifiedNames = ticket
            .Principal.GetCoreIdentityNames()
            .Select(n => n.NameParts.Select(part => part.Value).ToArray())
            .ToArray();
        var verifiedDatesOfBirth = ticket
            .Principal.GetCoreIdentityBirthDates()
            .Select(d => d.Value)
            .ToArray();
        var coreIdentityClaimVc =
            ticket.Principal.FindFirstValue("vc")
            ?? throw new InvalidOperationException("No vc claim present.");

        return OnUserVerifiedCore(
            journeyInstance,
            verifiedNames,
            verifiedDatesOfBirth,
            coreIdentityClaimVc,
            state => state.OneLoginAuthenticationTicket = ticket
        );
    }

    public async Task OnUserVerifiedCore(
        JourneyInstance<SignInJourneyState> journeyInstance,
        string[][] verifiedNames,
        DateOnly[] verifiedDatesOfBirth,
        string? coreIdentityClaimVc,
        Action<SignInJourneyState>? updateState = null
    )
    {
        var sub =
            journeyInstance.State.OneLoginAuthenticationTicket!.Principal.FindFirstValue("sub")
            ?? throw new InvalidOperationException("No sub claim.");

        var oneLoginUser = await dbContext.OneLoginUsers.SingleAsync(u => u.Subject == sub);
        oneLoginUser.VerifiedOn = clock.UtcNow;
        oneLoginUser.VerificationRoute = OneLoginUserVerificationRoute.OneLogin;
        oneLoginUser.VerifiedNames = verifiedNames;
        oneLoginUser.VerifiedDatesOfBirth = verifiedDatesOfBirth;
        oneLoginUser.LastCoreIdentityVc = coreIdentityClaimVc;

        if (await TryApplyLinkingToken(journeyInstance) is { } result)
        {
            oneLoginUser.PersonId = result.PersonId;
            oneLoginUser.FirstSignIn = clock.UtcNow;
            oneLoginUser.LastSignIn = clock.UtcNow;
            oneLoginUser.MatchRoute = OneLoginUserMatchRoute.LinkingToken;
        }

        await dbContext.SaveChangesAsync();

        await journeyInstance.UpdateStateAsync(state =>
        {
            updateState?.Invoke(state);
            state.AttemptedIdentityVerification = true;

            state.SetVerified(verifiedNames, verifiedDatesOfBirth);

            if (oneLoginUser.PersonId is not null)
            {
                Complete(state);
            }
        });
    }

    private async Task<TryApplyLinkingTokenResult?> TryApplyLinkingToken(
        JourneyInstance<SignInJourneyState> journeyInstance
    )
    {
        if (journeyInstance.State.LinkingToken is not { } linkingToken)
        {
            return null;
        }

        var personId = oneLoginAccountLinkingService.GetAccountIdForLinkingToken(linkingToken);

        if (personId is null)
        {
            return null;
        }

        var linkingTokenPerson = await dbContext.Persons.SingleOrDefaultAsync(p =>
            p.PersonId == personId
        );

        if (linkingTokenPerson is null)
        {
            return null;
        }

        // Invalidate the token
        oneLoginAccountLinkingService.InvalidateLinkingToken(linkingToken);

        return new TryApplyLinkingTokenResult(linkingTokenPerson.PersonId);
    }

    public async Task<IResult> OnVerificationFailed(
        JourneyInstance<SignInJourneyState> journeyInstance
    )
    {
        await journeyInstance.UpdateStateAsync(state =>
        {
            state.AttemptedIdentityVerification = true;
        });

        return GetNextPage(journeyInstance);
    }

    public IResult GetNextPage(JourneyInstance<SignInJourneyState> journeyInstance) =>
        journeyInstance.State switch
        {
            // Authentication is complete
            { AuthenticationTicket: not null } => Results.Redirect(
                GetSafeRedirectUri(journeyInstance)
            ),

            // Authenticated with OneLogin, identity verification succeeded, not yet matched to teaching record
            {
                OneLoginAuthenticationTicket: not null,
                IdentityVerified: true,
                AuthenticationTicket: null
            } => Results.Redirect(LinkGenerator.NotFound(journeyInstance.InstanceId)),

            // Authenticated with OneLogin, not yet verified
            {
                OneLoginAuthenticationTicket: not null,
                IdentityVerified: false,
                AttemptedIdentityVerification: false
            } => VerifyIdentityWithOneLogin(journeyInstance),

            // Authenticated with OneLogin, identity verification failed
            { OneLoginAuthenticationTicket: not null, IdentityVerified: false } => Results.Redirect(
                LinkGenerator.NotVerified(journeyInstance.InstanceId)
            ),

            _ => throw new InvalidOperationException("Cannot determine next page."),
        };

    public string GetSafeRedirectUri(JourneyInstance<SignInJourneyState> journeyInstance) =>
        EnsureUrlHasJourneyId(journeyInstance.State.RedirectUri, journeyInstance.InstanceId);

    public static void Complete(SignInJourneyState state)
    {
        if (state.OneLoginAuthenticationTicket is null)
        {
            throw new InvalidOperationException("User is not authenticated with One Login.");
        }

        var oneLoginPrincipal = state.OneLoginAuthenticationTicket.Principal;
        var oneLoginIdToken = state.OneLoginAuthenticationTicket.Properties.GetTokenValue(
            OpenIdConnectParameterNames.IdToken
        )!;

        var ecfIdentity = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Subject, oneLoginPrincipal.FindFirstValue("sub")!),
                new Claim(ClaimTypes.OneLoginIdToken, oneLoginIdToken),
            ],
            authenticationType: "Authorize access to an ECF account",
            nameType: "sub",
            roleType: null
        );

        var principal = new ClaimsPrincipal(ecfIdentity);

        state.AuthenticationTicket = new AuthenticationTicket(
            principal,
            properties: null,
            AuthenticationSchemes.MatchToEcfAccount
        );
    }

    private static string EnsureUrlHasJourneyId(string url, JourneyInstanceId instanceId)
    {
        var queryParamName = Constants.UniqueKeyQueryParameterName;

        return !url.Contains($"{queryParamName}=", StringComparison.Ordinal)
            ? QueryHelpers.AddQueryString(url, queryParamName, instanceId.UniqueKey!)
            : url;
    }

    private static IResult OneLoginChallenge(
        JourneyInstance<SignInJourneyState> journeyInstance,
        string vtr
    )
    {
        var delegatedProperties = new AuthenticationProperties();
        delegatedProperties.SetVectorOfTrust(vtr);
        delegatedProperties.Items.Add(
            FormFlowJourneySignInHandler.PropertyKeys.JourneyInstanceId,
            journeyInstance.InstanceId.Serialize()
        );
        return Results.Challenge(
            delegatedProperties,
            authenticationSchemes: [OneLoginDefaults.AuthenticationScheme]
        );
    }

    private record TryApplyLinkingTokenResult(Guid PersonId);
}
