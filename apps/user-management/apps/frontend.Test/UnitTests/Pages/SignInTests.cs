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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;
using ManageAccountsIndex = Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts.Index;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages;

public class SignInTests
{
    private SignIn Sut { get; }

    private readonly MockAuthServiceClient _authServiceClient = new();

    public SignInTests()
    {
        Sut = new SignIn(new FakeLinkGenerator(), _authServiceClient.Object);
    }

    [Fact]
    public void Get_WhenCalledAndEcswIsRegistered_RedirectsToManageAccounts()
    {
        // Arrange
        _authServiceClient.Setup(x => x.HttpContextService.GetIsEcswRegistered()).Returns(true);

        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<RedirectToActionResult>();
        var response = result as RedirectToActionResult;
        response.Should().NotBeNull();
        response!.ControllerName.Should().Be("Home");
        response.ActionName.Should().Be("Index");

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
}
