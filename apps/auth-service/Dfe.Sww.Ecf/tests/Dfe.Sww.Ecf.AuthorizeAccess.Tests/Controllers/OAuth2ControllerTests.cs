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
                scopes: $"{CustomScopes.EcswRegistered} profile"
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
                scopes: $"{CustomScopes.EcswRegistered} profile"
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
                scopes: $"{CustomScopes.EcswRegistered} profile"
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
        string scopes)
    {
        var appManager = new Mock<IOpenIddictApplicationManager>();
        var app = new object();
        appManager.Setup(m => m.FindByClientIdAsync(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync(app);

        var scopeManager = new Mock<IOpenIddictScopeManager>();
        scopeManager.Setup(m => m.ListResourcesAsync(It.IsAny<ImmutableArray<string>>(), It.IsAny<CancellationToken>()))
            .Returns((ImmutableArray<string> _, CancellationToken _) => AsyncEnumerable.Empty<string>());

        var userInstanceStateProvider = new Mock<IUserInstanceStateProvider>();

        var testJourneyState = new SignInJourneyState(
            redirectUri: "/test-redirect",
            serviceName: "Test Service",
            serviceUrl: "https://test.service",
            linkingToken: "test-linking-token");

        var testJourneyInstanceId = new JourneyInstanceId("test-instance", new Dictionary<string, StringValues>());

        var testJourneyInstance = (JourneyInstance<SignInJourneyState>)JourneyInstance.Create(
            userInstanceStateProvider.Object,
            testJourneyInstanceId,
            typeof(SignInJourneyState),
            testJourneyState,
            properties: new Dictionary<object, object>());

        userInstanceStateProvider
            .Setup(p => p.GetInstanceAsync(
                It.IsAny<JourneyInstanceId>(),
                It.IsAny<Type>()))
            .ReturnsAsync(testJourneyInstance);

        var journeyHelper = new SignInJourneyHelper(
            dbContext,
            Mock.Of<IOneLoginAccountLinkingService>(),
            Mock.Of<AuthorizeAccessLinkGenerator>(),
            Mock.Of<IOptions<AuthorizeAccessOptions>>(),
            userInstanceStateProvider.Object,
            Mock.Of<IClock>(),
            Mock.Of<IOptions<DatabaseSeedOptions>>()
        );

        var controller = new OAuth2Controller(dbContext, appManager.Object, scopeManager.Object, journeyHelper);

        var services = new ServiceCollection()
            .AddSingleton<IAuthenticationService>(new FakeAuthenticationService(subject))
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

        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        return Task.FromResult(controller);
    }
}
