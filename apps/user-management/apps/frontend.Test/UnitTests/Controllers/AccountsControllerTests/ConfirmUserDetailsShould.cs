using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Controllers.AccountsControllerTests;

public class ConfirmUserDetailsShould : AccountsControllerTestBase
{
    [Fact]
    public async Task Get_WhenCalled_LoadsTheView()
    {
        // Arrange
        var expectedAccount = AccountFaker.GenerateNewUser();
        Sut.SelectUserType(expectedAccount.Types!.First());
        await Sut.AddUserDetails(expectedAccount);

        // Act
        var result = Sut.ConfirmUserDetails();

        // Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = result as ViewResult;

        var requestHeader = viewResult!.ViewData["Referer"]?.ToString();
        requestHeader.Should().Be("test-referer");

        viewResult.ViewData.Model.Should().BeEquivalentTo(expectedAccount);
    }

    [Fact]
    public async Task Post_WhenCalled_RedirectsToAccountsIndex()
    {
        // Arrange
        var expectedAccount = AccountFaker.GenerateNewUser();

        Sut.SelectUserType(expectedAccount.Types!.First());
        await Sut.AddUserDetails(expectedAccount);

        // Act
        var result = Sut.ConfirmUserDetails_Post();

        // Assert
        result.Should().BeOfType<RedirectToActionResult>();

        var redirectToActionResult = result as RedirectToActionResult;
        redirectToActionResult!.ControllerName.Should().BeNull();
        redirectToActionResult.ActionName.Should().Be("Index");
    }
}
