using System.Diagnostics;
using Dfe.Sww.Ecf.Core.DataStore.Postgres;
using Dfe.Sww.Ecf.Core.Services.Accounts;
using GovUk.OneLogin.AspNetCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Tests;

public class SignInJourneyHelperTests(HostFixture hostFixture) : TestBase(hostFixture)
{
    [Fact]
    public Task OnOneLoginCallback_AuthenticationOnly_UserAlreadyExistsAndEcfAccountKnown_CompletesJourney() =>
        WithDbContext(async dbContext =>
        {
            // Arrange
            var helper = CreateHelper(dbContext);

            var person = await TestData.CreatePerson(b => b.WithTrn());
            var user = await TestData.CreateOneLoginUser(person);
            Clock.Advance();

            var state = new SignInJourneyState(
                redirectUri: "/",
                serviceName: "Test Service",
                serviceUrl: "https://service"
            );
            var journeyInstance = await CreateJourneyInstance(state);

            var authenticationTicket = CreateOneLoginAuthenticationTicket(
                vtr: SignInJourneyHelper.AuthenticationOnlyVtr,
                sub: user.Subject
            );

            // Act
            var result = await helper.OnOneLoginCallback(journeyInstance, authenticationTicket);

            // Assert
            Assert.NotNull(state.AuthenticationTicket);

            var oneLoginUser = await WithDbContext(ctx =>
                ctx.OneLoginUsers.SingleAsync(u => u.Subject == user.Subject)
            );
            Assert.NotEqual(Clock.UtcNow, oneLoginUser.FirstOneLoginSignIn);
            Assert.Equal(Clock.UtcNow, oneLoginUser.LastOneLoginSignIn);
            Assert.NotEqual(Clock.UtcNow, oneLoginUser.FirstSignIn);
            Assert.Equal(Clock.UtcNow, oneLoginUser.LastSignIn);

            var redirectResult = Assert.IsType<RedirectHttpResult>(result);
            Assert.Equal(
                $"{state.RedirectUri}?{journeyInstance.GetUniqueIdQueryParameter()}",
                redirectResult.Url
            );
        });

    [Fact]
    public Task OnOneLoginCallback_AuthenticationOnly_UserAlreadyExistsAndValidLinkingToken_LinksAccountAndCompletesJourney() =>
        WithDbContext(async dbContext =>
        {
            // Arrange
            var oneLoginAccountLinkingService =
                ActivatorUtilities.CreateInstance<OneLoginAccountLinkingService>(
                    HostFixture.Services
                );
            var helper = CreateHelper(dbContext);

            var person = await TestData.CreatePerson();
            var user = await TestData.CreateOneLoginUser(personId: null);
            Clock.Advance();

            var linkingToken = await oneLoginAccountLinkingService.GetLinkingTokenForAccountIdAsync(
                person.PersonId
            );

            var state = new SignInJourneyState(
                redirectUri: "/",
                serviceName: "Test Service",
                serviceUrl: "https://service",
                linkingToken
            );
            var journeyInstance = await CreateJourneyInstance(state);

            var authenticationTicket = CreateOneLoginAuthenticationTicket(
                vtr: SignInJourneyHelper.AuthenticationOnlyVtr,
                sub: user.Subject,
                email: user.Subject,
                createCoreIdentityVc: false
            );

            // Act
            var result = await helper.OnOneLoginCallback(journeyInstance, authenticationTicket);

            // Assert
            var redirectResult = Assert.IsType<RedirectHttpResult>(result);
            Assert.Equal(
                $"{state.RedirectUri}?{journeyInstance.GetUniqueIdQueryParameter()}",
                redirectResult.Url
            );

            var oneLoginUser = await WithDbContext(ctx =>
                ctx.OneLoginUsers.SingleAsync(u => u.Subject == user.Subject)
            );
            Assert.Equal(oneLoginUser.PersonId, person.PersonId);
        });

    [Fact]
    public Task OnOneLoginCallback_AuthenticationOnly_UserAlreadyExistsButEcfAccountNotKnown_RequestsIdentityVerification() =>
        WithDbContext(async dbContext =>
        {
            // Arrange
            var helper = CreateHelper(dbContext);

            var user = await TestData.CreateOneLoginUser(personId: null);
            Clock.Advance();

            var state = new SignInJourneyState(
                redirectUri: "/",
                serviceName: "Test Service",
                serviceUrl: "https://service"
            );
            var journeyInstance = await CreateJourneyInstance(state);

            var authenticationTicket = CreateOneLoginAuthenticationTicket(
                vtr: SignInJourneyHelper.AuthenticationOnlyVtr,
                sub: user.Subject
            );

            // Act
            var result = await helper.OnOneLoginCallback(journeyInstance, authenticationTicket);

            // Assert
            Assert.NotNull(state.OneLoginAuthenticationTicket);
            Assert.Null(state.AuthenticationTicket);

            var oneLoginUser = await WithDbContext(ctx =>
                ctx.OneLoginUsers.SingleAsync(u => u.Subject == user.Subject)
            );
            Assert.NotEqual(Clock.UtcNow, oneLoginUser.FirstOneLoginSignIn);
            Assert.Equal(Clock.UtcNow, oneLoginUser.LastOneLoginSignIn);
            Assert.NotEqual(Clock.UtcNow, oneLoginUser.FirstSignIn);
            Assert.NotEqual(Clock.UtcNow, oneLoginUser.LastSignIn);

            var challengeResult = Assert.IsType<ChallengeHttpResult>(result);
            Assert.Equal(
                SignInJourneyHelper.AuthenticationAndIdentityVerificationVtr,
                challengeResult.Properties?.GetVectorOfTrust()
            );
        });

    [Fact]
    public Task OnOneLoginCallback_AuthenticationOnly_UserDoesNotExist_RequestsIdentityVerification() =>
        WithDbContext(async dbContext =>
        {
            // Arrange
            var helper = CreateHelper(dbContext);

            var state = new SignInJourneyState(
                redirectUri: "/",
                serviceName: "Test Service",
                serviceUrl: "https://service"
            );
            var journeyInstance = await CreateJourneyInstance(state);

            var subject = TestData.CreateOneLoginUserSubject();
            var authenticationTicket = CreateOneLoginAuthenticationTicket(
                vtr: SignInJourneyHelper.AuthenticationOnlyVtr,
                sub: subject
            );

            // Act
            var result = await helper.OnOneLoginCallback(journeyInstance, authenticationTicket);

            // Assert
            Assert.NotNull(state.OneLoginAuthenticationTicket);
            Assert.Null(state.AuthenticationTicket);

            var user = await WithDbContext(ctx =>
                ctx.OneLoginUsers.SingleOrDefaultAsync(u => u.Subject == subject)
            );
            Assert.NotNull(user);
            Assert.Equal(Clock.UtcNow, user.FirstOneLoginSignIn);
            Assert.Null(user.FirstSignIn);
            Assert.Equal(Clock.UtcNow, user.LastOneLoginSignIn);
            Assert.NotEqual(Clock.UtcNow, user.LastSignIn);

            var challengeResult = Assert.IsType<ChallengeHttpResult>(result);
            Assert.Equal(
                SignInJourneyHelper.AuthenticationAndIdentityVerificationVtr,
                challengeResult.Properties?.GetVectorOfTrust()
            );
        });

    [Fact]
    public Task OnOneLoginCallback_AuthenticationOnly_UserDoesNotExistAndValidLinkingToken_LinksAccountAndCompletesJourney() =>
        WithDbContext(async dbContext =>
        {
            // Arrange
            var oneLoginAccountLinkingService =
                ActivatorUtilities.CreateInstance<OneLoginAccountLinkingService>(
                    HostFixture.Services
                );
            var helper = CreateHelper(dbContext);

            var person = await TestData.CreatePerson();
            Clock.Advance();

            var linkingToken = await oneLoginAccountLinkingService.GetLinkingTokenForAccountIdAsync(
                person.PersonId
            );

            var state = new SignInJourneyState(
                redirectUri: "/",
                serviceName: "Test Service",
                serviceUrl: "https://service",
                linkingToken
            );
            var journeyInstance = await CreateJourneyInstance(state);

            var subject = TestData.CreateOneLoginUserSubject();

            var authenticationTicket = CreateOneLoginAuthenticationTicket(
                vtr: SignInJourneyHelper.AuthenticationOnlyVtr,
                sub: subject
            );

            // Act
            var result = await helper.OnOneLoginCallback(journeyInstance, authenticationTicket);

            // Assert
            var redirectResult = Assert.IsType<RedirectHttpResult>(result);
            Assert.Equal(
                $"{state.RedirectUri}?{journeyInstance.GetUniqueIdQueryParameter()}",
                redirectResult.Url
            );

            var oneLoginUser = await WithDbContext(ctx =>
                ctx.OneLoginUsers.SingleAsync(u => u.Subject == subject)
            );
            Assert.NotNull(oneLoginUser);
            Assert.Equal(Clock.UtcNow, oneLoginUser.FirstOneLoginSignIn);
            Assert.Equal(Clock.UtcNow, oneLoginUser.FirstSignIn);
            Assert.Equal(Clock.UtcNow, oneLoginUser.LastOneLoginSignIn);
            Assert.Equal(Clock.UtcNow, oneLoginUser.LastSignIn);
            Assert.Equal(OneLoginUserMatchRoute.LinkingToken, oneLoginUser.MatchRoute);
            Assert.Equal(oneLoginUser.PersonId, person.PersonId);
        });

    [Fact]
    public Task OnOneLoginCallback_AuthenticationAndVerification_VerificationFailed_RedirectsToErrorPage() =>
        WithDbContext(async dbContext =>
        {
            // Arrange
            var helper = CreateHelper(dbContext);

            var user = await TestData.CreateOneLoginUser(personId: null);
            Clock.Advance();

            var state = new SignInJourneyState(
                redirectUri: "/",
                serviceName: "Test Service",
                serviceUrl: "https://service"
            );
            var journeyInstance = await CreateJourneyInstance(state);

            var authenticationTicket = CreateOneLoginAuthenticationTicket(
                vtr: SignInJourneyHelper.AuthenticationOnlyVtr,
                sub: user.Subject,
                email: user.Subject,
                createCoreIdentityVc: false
            );
            var callbackResult = await helper.OnOneLoginCallback(
                journeyInstance,
                authenticationTicket
            );
            Debug.Assert(callbackResult is ChallengeHttpResult);

            // Act
            var result = await helper.OnVerificationFailed(journeyInstance);

            // Assert
            Assert.NotNull(state.OneLoginAuthenticationTicket);
            Assert.Null(state.AuthenticationTicket);

            var oneLoginUser = await WithDbContext(ctx =>
                ctx.OneLoginUsers.SingleAsync(u => u.Subject == user.Subject)
            );
            Assert.NotEqual(Clock.UtcNow, oneLoginUser.FirstOneLoginSignIn);
            Assert.Null(oneLoginUser.FirstSignIn);
            Assert.Equal(Clock.UtcNow, oneLoginUser.LastOneLoginSignIn);
            Assert.NotEqual(Clock.UtcNow, oneLoginUser.LastSignIn);

            var redirectResult = Assert.IsType<RedirectHttpResult>(result);
            Assert.Equal(
                $"/NotVerified?{journeyInstance.GetUniqueIdQueryParameter()}",
                redirectResult.Url
            );
        });

    [Fact]
    public Task OnOneLoginCallback_AuthenticationAndVerification_VerificationSucceededWithoutLinkingTokenOrPersonId_RedirectsToNotFound() =>
        WithDbContext(async dbContext =>
        {
            // Arrange
            var helper = CreateHelper(dbContext);

            var user = await TestData.CreateOneLoginUser(personId: null);
            Clock.Advance();

            var firstName = Faker.Name.First();
            var lastName = Faker.Name.Last();
            var dateOfBirth = DateOnly.FromDateTime(Faker.Identification.DateOfBirth());

            var state = new SignInJourneyState(
                redirectUri: "/",
                serviceName: "Test Service",
                serviceUrl: "https://service"
            );
            var journeyInstance = await CreateJourneyInstance(state);

            var authenticationTicket = CreateOneLoginAuthenticationTicket(
                vtr: SignInJourneyHelper.AuthenticationOnlyVtr,
                sub: user.Subject,
                email: user.Subject,
                createCoreIdentityVc: false
            );
            var callbackResult = await helper.OnOneLoginCallback(
                journeyInstance,
                authenticationTicket
            );
            Debug.Assert(callbackResult is ChallengeHttpResult);

            var verifiedTicket = CreateOneLoginAuthenticationTicket(
                vtr: SignInJourneyHelper.AuthenticationAndIdentityVerificationVtr,
                sub: user.Subject,
                firstName: firstName,
                lastName: lastName,
                dateOfBirth: dateOfBirth,
                createCoreIdentityVc: true
            );

            // Act
            var result = await helper.OnOneLoginCallback(journeyInstance, verifiedTicket);

            // Assert
            Assert.NotNull(state.OneLoginAuthenticationTicket);
            Assert.Null(state.AuthenticationTicket);

            var oneLoginUser = await WithDbContext(ctx =>
                ctx.OneLoginUsers.SingleAsync(u => u.Subject == user.Subject)
            );
            Assert.NotEqual(Clock.UtcNow, oneLoginUser.FirstOneLoginSignIn);
            Assert.Null(oneLoginUser.FirstSignIn);
            Assert.Equal(Clock.UtcNow, oneLoginUser.LastOneLoginSignIn);
            Assert.NotEqual(Clock.UtcNow, oneLoginUser.LastSignIn);
            Assert.Equal(Clock.UtcNow, oneLoginUser.VerifiedOn);
            Assert.Equal(OneLoginUserVerificationRoute.OneLogin, oneLoginUser.VerificationRoute);
            Assert.NotNull(oneLoginUser.VerifiedNames);
            Assert.Collection(
                oneLoginUser.VerifiedNames,
                names =>
                    Assert.Collection(
                        names,
                        n => Assert.Equal(firstName, n),
                        n => Assert.Equal(lastName, n)
                    )
            );
            Assert.NotNull(oneLoginUser.VerifiedDatesOfBirth);
            Assert.Collection(
                oneLoginUser.VerifiedDatesOfBirth,
                dob => Assert.Equal(dateOfBirth, dob)
            );

            var redirectResult = Assert.IsType<RedirectHttpResult>(result);
            Assert.Equal(
                $"/NotFound?{journeyInstance.GetUniqueIdQueryParameter()}",
                redirectResult.Url
            );
        });

    [Fact]
    public Task OnOneLoginCallback_AuthenticationAndVerification_VerificationSucceededButLinkingTokenInvalid_RedirectsToNotFound() =>
        WithDbContext(async dbContext =>
        {
            // Arrange
            var helper = CreateHelper(dbContext);

            var person = await TestData.CreatePerson(b => b.WithTrn());
            var user = await TestData.CreateOneLoginUser(personId: null);
            Clock.Advance();

            const string linkingToken = "invalid-linking-token";

            var firstName = person.FirstName;
            var lastName = person.LastName;
            var dateOfBirth = person.DateOfBirth;

            var state = new SignInJourneyState(
                redirectUri: "/",
                serviceName: "Test Service",
                serviceUrl: "https://service",
                linkingToken
            );
            var journeyInstance = await CreateJourneyInstance(state);

            var authenticationTicket = CreateOneLoginAuthenticationTicket(
                vtr: SignInJourneyHelper.AuthenticationOnlyVtr,
                sub: user.Subject,
                email: user.Subject,
                createCoreIdentityVc: false
            );
            var callbackResult = await helper.OnOneLoginCallback(
                journeyInstance,
                authenticationTicket
            );
            Debug.Assert(callbackResult is ChallengeHttpResult);

            var verifiedTicket = CreateOneLoginAuthenticationTicket(
                vtr: SignInJourneyHelper.AuthenticationAndIdentityVerificationVtr,
                sub: user.Subject,
                firstName: firstName,
                lastName: lastName,
                dateOfBirth: dateOfBirth,
                createCoreIdentityVc: true
            );

            // Act
            var result = await helper.OnOneLoginCallback(journeyInstance, verifiedTicket);

            // Assert
            var redirectResult = Assert.IsType<RedirectHttpResult>(result);
            Assert.Equal(
                $"/NotFound?{journeyInstance.GetUniqueIdQueryParameter()}",
                redirectResult.Url
            );
        });

    private SignInJourneyHelper CreateHelper(EcfDbContext dbContext)
    {
        var linkGenerator = new FakeLinkGenerator();
        var options = Options.Create(new AuthorizeAccessOptions() { ShowDebugPages = false });

        return ActivatorUtilities.CreateInstance<SignInJourneyHelper>(
            HostFixture.Services,
            dbContext,
            linkGenerator,
            options,
            Clock
        );
    }

    private class FakeLinkGenerator : AuthorizeAccessLinkGenerator
    {
        protected override string GetRequiredPathByPage(
            string page,
            string? handler = null,
            object? routeValues = null
        ) => page;
    }
}
