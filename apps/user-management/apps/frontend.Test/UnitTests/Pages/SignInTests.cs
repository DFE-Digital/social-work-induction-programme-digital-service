using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Pagination;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Builders;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Services;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageAccounts;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;
using ManageAccountsIndex = Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts.Index;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages;

public class SignInTests: PageModelTestBase<SignIn>
{
    private SignIn Sut { get; }

    private readonly MockAuthServiceClient _authServiceClient = new();

    public SignInTests()
    {
        Sut = new SignIn(new FakeLinkGenerator(), _authServiceClient.Object)
        {
            PageContext =
            {
                HttpContext = HttpContext
            }
        };
    }

    [Fact]
    public void Get_WhenCalledAndEcswIsRegistered_RedirectsToManageAccounts()
    {
        // Arrange
        _authServiceClient.Setup(x => x.HttpContextService.GetIsEcswRegistered()).Returns(true);

        // Act
        var result = Sut.OnGet();

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
    public void Get_WhenCalledAndEcswIsNotRegistered_RedirectsToSocialWorkerRegistrationStart()
    {
        // Arrange
        _authServiceClient.Setup(x => x.HttpContextService.GetIsEcswRegistered()).Returns(false);

        // Act
        var result = Sut.OnGet();

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
    public void Get_WhenCalledAndUserIsAdministrator_RedirectsToDashboard()
    {
        // Arrange
        _authServiceClient.Setup(x => x.HttpContextService.GetIsEcswRegistered()).Returns(true);
        HttpContext.User = new ClaimsPrincipalBuilder().WithRole(RoleType.Administrator).Build();

        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var response = result as RedirectResult;
        response.Should().NotBeNull();
        response!.Url.Should().Be("/dashboard");
    }
}
