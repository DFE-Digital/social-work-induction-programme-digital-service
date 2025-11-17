using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Pagination;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Email.Models;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Builders;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages;

public class SignInTests: PageModelTestBase<SignIn>
{
    private SignIn Sut { get; }

    private readonly MockAuthServiceClient _authServiceClient = new();

    public SignInTests()
    {
        Sut = new SignIn(new FakeLinkGenerator(), _authServiceClient.Object, MockEmailService.Object)
        {
            PageContext =
            {
                HttpContext = HttpContext
            }
        };
    }

    [Fact]
    public async Task GetAsync_WhenCalledAndEcswIsRegistered_RedirectsToManageAccounts()
    {
        // Arrange
        HttpContext.User = new ClaimsPrincipalBuilder().WithRole(RoleType.EarlyCareerSocialWorker).Build();
        _authServiceClient.Setup(x => x.HttpContextService.GetIsEcswRegistered()).Returns(true);

        // Act
        var result = await Sut.OnGetAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var response = result as RedirectResult;
        response.Should().NotBeNull();
        response!.Url.Should().Be("index");

        _authServiceClient.Verify(
            x =>
                x.HttpContextService.GetIsEcswRegistered(),
            Times.Once
        );
    }

    [Fact]
    public async Task GetAsync_WhenCalledAndEcswIsNotRegistered_RedirectsToSocialWorkerRegistrationStart()
    {
        // Arrange
        HttpContext.User = new ClaimsPrincipalBuilder().WithRole(RoleType.EarlyCareerSocialWorker).Build();
        _authServiceClient.Setup(x => x.HttpContextService.GetIsEcswRegistered()).Returns(false);

        // Act
        var result = await Sut.OnGetAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var response = result as RedirectResult;
        response.Should().NotBeNull();
        response!.Url.Should().Be("/social-worker-registration");

        _authServiceClient.Verify(
            x =>
                x.HttpContextService.GetIsEcswRegistered(),
            Times.Once
        );
    }

    [Fact]
    public async Task Get_WhenCalledAndUserIsAdministrator_RedirectsToDashboard()
    {
        // Arrange
        HttpContext.User = new ClaimsPrincipalBuilder().WithRole(RoleType.Administrator).Build();
        _authServiceClient.Setup(x => x.HttpContextService.GetIsEcswRegistered()).Returns(true);

        // Act
        var result = await Sut.OnGetAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var response = result as RedirectResult;
        response.Should().NotBeNull();
        response!.Url.Should().Be("/dashboard");
    }

    [Fact]
    public async Task GetAsync_WhenCalledAndStaffFirstLogin_CallsSendWelcomeEmail()
    {
        // Arrange
        HttpContext.User = new ClaimsPrincipalBuilder().WithRole(RoleType.Assessor).Build();
        _authServiceClient.Setup(x => x.HttpContextService.GetIsStaffFirstLogin()).Returns(true);

        // Act
        var result = await Sut.OnGetAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var response = result as RedirectResult;
        response.Should().NotBeNull();
        response!.Url.Should().Be("index");

        _authServiceClient.Verify(
            x =>
                x.HttpContextService.GetIsStaffFirstLogin(),
            Times.Once
        );
        MockEmailService.Verify(x => x.SendWelcomeEmailAsync(
            It.Is<WelcomeEmailRequest>(req => req.AccountId == _authServiceClient.Object.HttpContextService.GetPersonId())
        ), Times.Once);
    }
}
