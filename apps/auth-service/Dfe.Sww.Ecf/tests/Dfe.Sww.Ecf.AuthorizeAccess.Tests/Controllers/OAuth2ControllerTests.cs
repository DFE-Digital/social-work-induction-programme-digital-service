using System.Collections.Immutable;
using Dfe.Sww.Ecf.AuthorizeAccess.Controllers;
using Dfe.Sww.Ecf.AuthorizeAccess.Infrastructure.FormFlow;
using Dfe.Sww.Ecf.AuthorizeAccess.Tests.Fakers;
using Dfe.Sww.Ecf.Core.DataStore.Postgres;
using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using Dfe.Sww.Ecf.Core.Infrastructure.Configuration;
using Dfe.Sww.Ecf.Core.Services.Accounts;
using Dfe.Sww.Ecf.UiCommon.FormFlow;
using Dfe.Sww.Ecf.UiCommon.FormFlow.State;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using OpenIddict.Abstractions;
using OpenIddict.Server;
using OpenIddict.Server.AspNetCore;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Tests.Controllers;

[Collection("Uses Database")]
public class OAuth2ControllerTests(HostFixture hostFixture) : TestBase(hostFixture)
{
    [Fact]
    public Task Authorize_WhenPendingEcsw_AddsIsEcswRegisteredFalse()
    {
        return WithDbContext(async dbContext =>
        {
            // Arrange
            var person = await TestData.CreatePerson(
                b => b.WithStatus(PersonStatus.PendingRegistration).WithRoles([RoleType.EarlyCareerSocialWorker]));

            var oneLoginUser = await TestData.CreateOneLoginUser(person);

            var controller = await CreateControllerWithContext(
                dbContext,
                subject: oneLoginUser.Subject,
                scopes: $"{CustomScopes.EcswRegistered} {OpenIddictConstants.Scopes.Profile}"
            );

            // Act
            var result = await controller.Authorize();

            // Assert
            var signIn = Assert.IsType<SignInResult>(result);
            var claim = signIn.Principal.Claims.SingleOrDefault(c => c.Type == ClaimTypes.IsEcswRegistered);
            Assert.NotNull(claim);
            Assert.Equal("false", claim.Value);
        });
    }

    [Fact]
    public Task Authorize_WhenNotEcswAndPending_DoesNotAddIsEcswRegistered()
    {
        return WithDbContext(async dbContext =>
        {
            // Arrange
            var person = await TestData.CreatePerson(b => b.WithStatus(PersonStatus.PendingRegistration).WithRoles([RoleType.Assessor]));

            var user = await TestData.CreateOneLoginUser(person);

            var controller = await CreateControllerWithContext(
                dbContext,
                subject: user.Subject,
                scopes: $"{CustomScopes.EcswRegistered} {OpenIddictConstants.Scopes.Profile}"
            );

            // Act
            var result = await controller.Authorize();

            // Assert
            var signIn = Assert.IsType<SignInResult>(result);
            Assert.DoesNotContain(signIn.Principal.Claims, c => c.Type == ClaimTypes.IsEcswRegistered);
        });
    }

    [Fact]
    public Task Authorize_WhenEcswAndNotPending_DoesNotAddIsEcswRegistered()
    {
        return WithDbContext(async dbContext =>
        {
            // Arrange
            var person = await TestData.CreatePerson(b => b.WithStatus(PersonStatus.Active).WithRoles([RoleType.EarlyCareerSocialWorker]));

            var user = await TestData.CreateOneLoginUser(person);

            var controller = await CreateControllerWithContext(
                dbContext,
                subject: user.Subject,
                scopes: $"{CustomScopes.EcswRegistered} {OpenIddictConstants.Scopes.Profile}"
            );

            // Act
            var result = await controller.Authorize();

            // Assert
            var signIn = Assert.IsType<SignInResult>(result);
            Assert.DoesNotContain(signIn.Principal.Claims, c => c.Type == ClaimTypes.IsEcswRegistered);
        });
    }

    [Fact]
    public Task Authorize_WhenScopePresentAndStateTrue_AddsIsStaffFirstLogin()
    {
        return WithDbContext(async dbContext =>
        {
            // Arrange
            var person = await TestData.CreatePerson(b => b.WithStatus(PersonStatus.Active).WithRoles([RoleType.EarlyCareerSocialWorker]));

            var user = await TestData.CreateOneLoginUser(person);

            var controller = await CreateControllerWithContext(
                dbContext,
                subject: user.Subject,
                scopes: $"{CustomScopes.StaffFirstLogin} {OpenIddictConstants.Scopes.Profile}",
                isStaffFirstLogin: true
            );

            // Act
            var result = await controller.Authorize();

            // Assert
            var signIn = Assert.IsType<SignInResult>(result);
            Assert.DoesNotContain(signIn.Principal.Claims, c => c.Type == ClaimTypes.IsEcswRegistered);
        });
    }

    [Fact]
    public Task Authorize_WhenStateFalse_DoesNotAddIsStaffFirstLogin()
    {
        return WithDbContext(async dbContext =>
        {
            // Arrange
            var person = await TestData.CreatePerson(b => b.WithStatus(PersonStatus.Active).WithRoles([RoleType.EarlyCareerSocialWorker]));

            var user = await TestData.CreateOneLoginUser(person);

            var controller = await CreateControllerWithContext(
                dbContext,
                subject: user.Subject,
                scopes: $"{CustomScopes.StaffFirstLogin} {OpenIddictConstants.Scopes.Profile}"
            );

            // Act
            var result = await controller.Authorize();

            // Assert
            var signIn = Assert.IsType<SignInResult>(result);
            Assert.DoesNotContain(signIn.Principal.Claims, c => c.Type == ClaimTypes.IsEcswRegistered);
        });
    }

    private Task<OAuth2Controller> CreateControllerWithContext(
        EcfDbContext dbContext,
        string subject,
        string scopes,
        bool isStaffFirstLogin = false)
    {
        var appManager = new Mock<IOpenIddictApplicationManager>();
        var app = new object();
        appManager
            .Setup(m => m.FindByClientIdAsync(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync(app);

        var scopeManager = new Mock<IOpenIddictScopeManager>();
        scopeManager
            .Setup(m => m.ListResourcesAsync(It.IsAny<ImmutableArray<string>>(), It.IsAny<CancellationToken>()))
            .Returns((ImmutableArray<string> _, CancellationToken _) => AsyncEnumerable.Empty<string>());

        var mockUserStateProvider = new Mock<IUserInstanceStateProvider>();

        var journeyState = new SignInJourneyState(
            redirectUri: "/test-redirect",
            serviceName: "Test Service",
            serviceUrl: "https://test.service",
            linkingToken: "test-linking-token")
        {
            IsStaffFirstLogin = isStaffFirstLogin
        };

        var journeyInstanceId = new JourneyInstanceId("test-instance", new Dictionary<string, StringValues>());

        var journeyInstance = (JourneyInstance<SignInJourneyState>)JourneyInstance.Create(
            mockUserStateProvider.Object,
            journeyInstanceId,
            typeof(SignInJourneyState),
            journeyState,
            properties: new Dictionary<object, object>());

        mockUserStateProvider
            .Setup(p => p.GetInstanceAsync(
                journeyInstanceId,
                typeof(SignInJourneyState)))
            .ReturnsAsync(journeyInstance);

        var services = new ServiceCollection()
            .AddSingleton<IAuthenticationService>(new FakeAuthenticationService(subject))
            .AddSingleton(mockUserStateProvider.Object)
            .AddSingleton(Mock.Of<IOneLoginAccountLinkingService>())
            .AddSingleton<AuthorizeAccessLinkGenerator, FakeLinkGenerator>()
            .AddSingleton<IClock>(Clock)
            .Configure<AuthorizeAccessOptions>(o => o.ShowDebugPages = false)
            .Configure<DatabaseSeedOptions>(_ => { })
            .BuildServiceProvider();

        var httpContext = new DefaultHttpContext { RequestServices = services };

        var oidcRequest = new OpenIddictRequest
        {
            ClientId = "test-client",
            RedirectUri = "https://test.service/callback",
            Scope = scopes,
            ResponseType = OpenIddictConstants.ResponseTypes.Code
        };

        httpContext.Features.Set(new OpenIddictServerAspNetCoreFeature
        {
            Transaction = new OpenIddictServerTransaction
            {
                Request = oidcRequest
            }
        });

        var journeyHelper = ActivatorUtilities.CreateInstance<SignInJourneyHelper>(services, dbContext, Clock);

        var controller = new OAuth2Controller(dbContext, appManager.Object, scopeManager.Object, journeyHelper);

        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        return Task.FromResult(controller);
    }
}
