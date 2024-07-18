using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using Dfe.Sww.Ecf.Frontend.Views.Accounts;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Controllers.AccountsControllerTests;

public class AddUserDetailsShould : AccountsControllerTestBase
{
    [Fact]
    public void Get_WhenCalled_LoadsTheView()
    {
        // Act
        var result = Sut.AddUserDetails();

        // Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = result as ViewResult;

        var requestHeader = viewResult!.ViewData["Referer"]?.ToString();
        requestHeader.Should().Be("test-referer");
    }

    [Fact]
    public async Task Post_WhenCalled_RedirectsToConfirmUserDetails()
    {
        // Arrange
        var account = AccountFaker.GenerateNewUser();
        var userDetailsModel = AddUserDetailsModel.FromAccount(account);

        // Act
        var result = await Sut.AddUserDetails(userDetailsModel);

        // Assert
        result.Should().BeOfType<RedirectToActionResult>();

        var redirectToActionResult = result as RedirectToActionResult;
        redirectToActionResult!.ControllerName.Should().BeNull();
        redirectToActionResult.ActionName.Should().Be("ConfirmUserDetails");
    }

    [Fact]
    public async Task Post_WhenCalledWithInvalidData_ReturnsErrorsAndRedirectsToAddUserDetails()
    {
        // Arrange
        var account = AccountFaker.GenerateNewUser();
        var userDetailsModel = new AddUserDetailsModel
        {
            FirstName = account.FirstName!,
            LastName = account.LastName!,
            Email = string.Empty
        };

        // Act
        var result = await Sut.AddUserDetails(userDetailsModel);

        // Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = result as ViewResult;
        viewResult!.ViewName.Should().Be("AddUserDetails");

        var modelState = viewResult.ViewData.ModelState;
        modelState.Keys.Count().Should().Be(1);
        modelState.Keys.Should().Contain("Email");
        modelState["Email"]!.Errors.Count.Should().Be(1);
        modelState["Email"]!.Errors[0].ErrorMessage.Should().Be("Enter an email");
    }
}
