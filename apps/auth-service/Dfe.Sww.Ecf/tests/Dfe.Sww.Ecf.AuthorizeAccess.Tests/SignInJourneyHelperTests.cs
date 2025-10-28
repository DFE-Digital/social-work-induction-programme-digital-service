using System.Diagnostics;
using Dfe.Sww.Ecf.Core.DataStore.Postgres;
using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using Dfe.Sww.Ecf.Core.Services.Accounts;
using Dfe.Sww.Ecf.TestCommon.Fakers;
using GovUk.OneLogin.AspNetCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Tests;

[Collection("Uses Database")]
public class SignInJourneyHelperTests(HostFixture hostFixture) : TestBase(hostFixture)
{
    [Fact]
    public Task OnOneLoginCallback_AuthenticationOnly_UserAlreadyExistsAndEcfAccountKnown_CompletesJourney()
    {
        return WithDbContext(async dbContext =>
        {
            // Arrange
            var helper = CreateHelper(dbContext);

            var person = await TestData.CreatePerson(b => b.WithTrn());
            var user = await TestData.CreateOneLoginUser(person);
            Clock.Advance();

            var state = new SignInJourneyState(
                "/",
                "Test Service",
                "https://service"
            );
            var journeyInstance = await CreateJourneyInstance(state);

            var authenticationTicket = CreateOneLoginAuthenticationTicket(
                SignInJourneyHelper.AuthenticationOnlyVtr,
                user.Subject
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
    }

    [Fact]
    public Task
        OnOneLoginCallback_AuthenticationOnly_UserAlreadyExistsAndValidLinkingToken_LinksAccountAndCompletesJourney()
    {
        return WithDbContext(async dbContext =>
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
                "/",
                "Test Service",
                "https://service",
                linkingToken
            );
            var journeyInstance = await CreateJourneyInstance(state);

            var authenticationTicket = CreateOneLoginAuthenticationTicket(
                SignInJourneyHelper.AuthenticationOnlyVtr,
                user.Subject,
                user.Subject,
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
    }

    [Fact]
    public Task
        OnOneLoginCallback_AuthenticationOnly_UserAlreadyExistsButEcfAccountNotKnown_RequestsIdentityVerification()
    {
        return WithDbContext(async dbContext =>
        {
            // Arrange
            var helper = CreateHelper(dbContext);

            var user = await TestData.CreateOneLoginUser(personId: null);
            Clock.Advance();

            var state = new SignInJourneyState(
                "/",
                "Test Service",
                "https://service"
            );
            var journeyInstance = await CreateJourneyInstance(state);

            var authenticationTicket = CreateOneLoginAuthenticationTicket(
                SignInJourneyHelper.AuthenticationOnlyVtr,
                user.Subject
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
    }

    [Fact]
    public Task OnOneLoginCallback_AuthenticationOnly_UserDoesNotExist_RequestsIdentityVerification()
    {
        return WithDbContext(async dbContext =>
        {
            // Arrange
            var helper = CreateHelper(dbContext);

            var state = new SignInJourneyState(
                "/",
                "Test Service",
                "https://service"
            );
            var journeyInstance = await CreateJourneyInstance(state);

            var subject = TestData.CreateOneLoginUserSubject();
            var authenticationTicket = CreateOneLoginAuthenticationTicket(
                SignInJourneyHelper.AuthenticationOnlyVtr,
                subject
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
    }

    [Fact]
    public Task
        OnOneLoginCallback_AuthenticationOnly_UserDoesNotExistAndValidLinkingToken_LinksAccountAndCompletesJourney()
    {
        return WithDbContext(async dbContext =>
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
                "/",
                "Test Service",
                "https://service",
                linkingToken
            );
            var journeyInstance = await CreateJourneyInstance(state);

            var subject = TestData.CreateOneLoginUserSubject();

            var authenticationTicket = CreateOneLoginAuthenticationTicket(
                SignInJourneyHelper.AuthenticationOnlyVtr,
                subject
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
    }

    [Fact]
    public Task OnOneLoginCallback_AuthenticationAndVerification_VerificationFailed_RedirectsToErrorPage()
    {
        return WithDbContext(async dbContext =>
        {
            // Arrange
            var helper = CreateHelper(dbContext);

            var user = await TestData.CreateOneLoginUser(personId: null);
            Clock.Advance();

            var state = new SignInJourneyState(
                "/",
                "Test Service",
                "https://service"
            );
            var journeyInstance = await CreateJourneyInstance(state);

            var authenticationTicket = CreateOneLoginAuthenticationTicket(
                SignInJourneyHelper.AuthenticationOnlyVtr,
                user.Subject,
                user.Subject,
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
    }

    [Fact]
    public Task
        OnOneLoginCallback_AuthenticationAndVerification_VerificationSucceededWithoutLinkingTokenOrPersonId_RedirectsToNotFound()
    {
        return WithDbContext(async dbContext =>
        {
            // Arrange
            var helper = CreateHelper(dbContext);

            var user = await TestData.CreateOneLoginUser(personId: null);
            Clock.Advance();

            var person = new PersonFaker().Generate();

            var state = new SignInJourneyState(
                "/",
                "Test Service",
                "https://service"
            );
            var journeyInstance = await CreateJourneyInstance(state);

            var authenticationTicket = CreateOneLoginAuthenticationTicket(
                SignInJourneyHelper.AuthenticationOnlyVtr,
                user.Subject,
                user.Subject,
                createCoreIdentityVc: false
            );
            var callbackResult = await helper.OnOneLoginCallback(
                journeyInstance,
                authenticationTicket
            );
            Debug.Assert(callbackResult is ChallengeHttpResult);

            var verifiedTicket = CreateOneLoginAuthenticationTicket(
                SignInJourneyHelper.AuthenticationAndIdentityVerificationVtr,
                user.Subject,
                firstName: person.FirstName,
                lastName: person.LastName,
                dateOfBirth: person.DateOfBirth,
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
                        n => Assert.Equal(person.FirstName, n),
                        n => Assert.Equal(person.LastName, n)
                    )
            );
            Assert.NotNull(oneLoginUser.VerifiedDatesOfBirth);
            Assert.Single(oneLoginUser.VerifiedDatesOfBirth, person.DateOfBirth);

            var redirectResult = Assert.IsType<RedirectHttpResult>(result);
            Assert.Equal(
                $"/NotFound?{journeyInstance.GetUniqueIdQueryParameter()}",
                redirectResult.Url
            );
        });
    }

    [Fact]
    public Task
        OnOneLoginCallback_AuthenticationAndVerification_VerificationSucceededButLinkingTokenInvalid_RedirectsToNotFound()
    {
        return WithDbContext(async dbContext =>
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
                "/",
                "Test Service",
                "https://service",
                linkingToken
            );
            var journeyInstance = await CreateJourneyInstance(state);

            var authenticationTicket = CreateOneLoginAuthenticationTicket(
                SignInJourneyHelper.AuthenticationOnlyVtr,
                user.Subject,
                user.Subject,
                createCoreIdentityVc: false
            );
            var callbackResult = await helper.OnOneLoginCallback(
                journeyInstance,
                authenticationTicket
            );
            Debug.Assert(callbackResult is ChallengeHttpResult);

            var verifiedTicket = CreateOneLoginAuthenticationTicket(
                SignInJourneyHelper.AuthenticationAndIdentityVerificationVtr,
                user.Subject,
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
    }

    [Fact]
    public Task OnUserAuthenticated_FirstLogin_WithStaffRole_SetsPersonActive()
    {
        return WithDbContext(async dbContext =>
        {
            // Arrange
            var oneLoginAccountLinkingService =
                ActivatorUtilities.CreateInstance<OneLoginAccountLinkingService>(
                    HostFixture.Services
                );
            var helper = CreateHelper(dbContext);

            var person = await TestData.CreatePerson(p => p.WithStatus(PersonStatus.PendingRegistration).WithRoles([RoleType.Assessor]));
            var user = await TestData.CreateOneLoginUser(personId: null);

            Clock.Advance();

            var linkingToken = await oneLoginAccountLinkingService.GetLinkingTokenForAccountIdAsync(
                person.PersonId
            );

            var state = new SignInJourneyState(
                "/",
                "Test Service",
                "https://service",
                linkingToken
            );
            var journeyInstance = await CreateJourneyInstance(state);

            var authenticationTicket = CreateOneLoginAuthenticationTicket(
                SignInJourneyHelper.AuthenticationOnlyVtr,
                user.Subject,
                user.Subject,
                createCoreIdentityVc: false
            );

            // Act
            await helper.OnOneLoginCallback(journeyInstance, authenticationTicket);

            // Assert
            var updatedPerson = await WithDbContext(ctx =>
                ctx.Persons.SingleAsync(p => p.PersonId == person.PersonId)
            );
            Assert.Equal(PersonStatus.Active, updatedPerson.Status);
        });
    }

    [Fact]
    public Task OnUserAuthenticated_FirstLogin_WithEarlyCareerSocialWorkerRole_DoesNotSetActive()
    {
        return WithDbContext(async dbContext =>
        {
            // Arrange
            var oneLoginAccountLinkingService =
                ActivatorUtilities.CreateInstance<OneLoginAccountLinkingService>(
                    HostFixture.Services
                );
            var helper = CreateHelper(dbContext);

            var person = await TestData.CreatePerson(p => p.WithStatus(PersonStatus.PendingRegistration).WithRoles([RoleType.EarlyCareerSocialWorker]));
            var user = await TestData.CreateOneLoginUser(personId: null);

            var linkingToken = await oneLoginAccountLinkingService.GetLinkingTokenForAccountIdAsync(person.PersonId);

            var state = new SignInJourneyState("/", "Test Service", "https://service", linkingToken);
            var journeyInstance = await CreateJourneyInstance(state);

            var authenticationTicket = CreateOneLoginAuthenticationTicket(
                SignInJourneyHelper.AuthenticationOnlyVtr,
                user.Subject,
                user.Subject,
                createCoreIdentityVc: false
            );

            // Act
            await helper.OnOneLoginCallback(journeyInstance, authenticationTicket);

            // Assert
            var updatedPerson = await WithDbContext(ctx =>
                ctx.Persons.SingleAsync(p => p.PersonId == person.PersonId)
            );
            Assert.Equal(PersonStatus.PendingRegistration, updatedPerson.Status);
        });
    }

    [Fact]
    public Task OnUserVerifiedCore_FirstLogin_WithStaffRole_SetsPersonActive()
    {
        return WithDbContext(async dbContext =>
        {
            // Arrange
            var oneLoginAccountLinkingService =
                ActivatorUtilities.CreateInstance<OneLoginAccountLinkingService>(HostFixture.Services);
            var helper = CreateHelper(dbContext);

            var person = await TestData.CreatePerson(p => p.WithStatus(PersonStatus.PendingRegistration).WithRoles([RoleType.Assessor]));
            var user = await TestData.CreateOneLoginUser(personId: null);

            var linkingToken = await oneLoginAccountLinkingService.GetLinkingTokenForAccountIdAsync(person.PersonId);

            var state = new SignInJourneyState("/", "Test Service", "https://service", linkingToken);
            var journeyInstance = await CreateJourneyInstance(state);

            var authOnlyTicket = CreateOneLoginAuthenticationTicket(
                SignInJourneyHelper.AuthenticationOnlyVtr,
                user.Subject,
                user.Subject,
                createCoreIdentityVc: false
            );
            await helper.OnOneLoginCallback(journeyInstance, authOnlyTicket);

            var verifiedTicket = CreateOneLoginAuthenticationTicket(
                SignInJourneyHelper.AuthenticationAndIdentityVerificationVtr,
                user.Subject,
                firstName: person.FirstName,
                lastName: person.LastName,
                dateOfBirth: person.DateOfBirth,
                createCoreIdentityVc: true
            );

            // Act
            await helper.OnOneLoginCallback(journeyInstance, verifiedTicket);

            // Assert
            var updatedPerson = await WithDbContext(ctx =>
                ctx.Persons.SingleAsync(p => p.PersonId == person.PersonId)
            );
            Assert.Equal(PersonStatus.Active, updatedPerson.Status);
        });
    }

    [Fact]
    public Task OnUserVerifiedCore_FirstLogin_WithEarlyCareerSocialWorkerRole_DoesNotSetActive()
    {
        return WithDbContext(async dbContext =>
        {
            // Arrange
            var oneLoginAccountLinkingService =
                ActivatorUtilities.CreateInstance<OneLoginAccountLinkingService>(HostFixture.Services);
            var helper = CreateHelper(dbContext);

            var person = await TestData.CreatePerson(p => p.WithStatus(PersonStatus.PendingRegistration).WithRoles([RoleType.EarlyCareerSocialWorker]));
            var user = await TestData.CreateOneLoginUser(personId: null);

            var linkingToken = await oneLoginAccountLinkingService.GetLinkingTokenForAccountIdAsync(person.PersonId);

            var state = new SignInJourneyState("/", "Test Service", "https://service", linkingToken);
            var journeyInstance = await CreateJourneyInstance(state);

            var authOnlyTicket = CreateOneLoginAuthenticationTicket(
                SignInJourneyHelper.AuthenticationOnlyVtr,
                user.Subject,
                user.Subject,
                createCoreIdentityVc: false
            );
            await helper.OnOneLoginCallback(journeyInstance, authOnlyTicket);

            var verifiedTicket = CreateOneLoginAuthenticationTicket(
                SignInJourneyHelper.AuthenticationAndIdentityVerificationVtr,
                user.Subject,
                firstName: person.FirstName,
                lastName: person.LastName,
                dateOfBirth: person.DateOfBirth,
                createCoreIdentityVc: true
            );

            // Act
            await helper.OnOneLoginCallback(journeyInstance, verifiedTicket);

            // Assert
            var updatedPerson = await WithDbContext(ctx =>
                ctx.Persons.SingleAsync(p => p.PersonId == person.PersonId)
            );
            Assert.Equal(PersonStatus.PendingRegistration, updatedPerson.Status);
        });
    }

    private SignInJourneyHelper CreateHelper(EcfDbContext dbContext)
    {
        var linkGenerator = new FakeLinkGenerator();
        var options = Options.Create(new AuthorizeAccessOptions { ShowDebugPages = false });

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
        )
        {
            return page;
        }
    }
}
