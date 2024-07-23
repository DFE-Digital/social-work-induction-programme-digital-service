using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using Dfe.Sww.Ecf.Frontend.Views.Accounts;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Controllers.AccountsControllerTests;

public class ConfirmUserDetailsShould : AccountsControllerTestBase
{
    [Fact]
    public void Get_WhenCalled_LoadsTheView()
    {
        // Arrange
        var account = AccountFaker.GenerateNewUser();
        var expectedUserDetails = AddAccountDetailsModel.FromAccount(account);
        CreateAccountJourneyService.SetAccountDetails(expectedUserDetails);

        // Act
        var result = Sut.ConfirmUserDetails();

        // Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = result as ViewResult;

        var requestHeader = viewResult!.ViewData["Referer"]?.ToString();
        requestHeader.Should().Be("test-referer");

        viewResult.ViewData.Model.Should().BeEquivalentTo(expectedUserDetails);
    }

    [Fact]
    public void Post_WhenCalled_RedirectsToAccountsIndex()
    {
        // Arrange
        var account = AccountFaker.GenerateNewUser();

        CreateAccountJourneyService.SetAccountTypes(new List<AccountType> { account.Types!.First() });
        CreateAccountJourneyService.SetAccountDetails(AddAccountDetailsModel.FromAccount(account));

        // Act
        var result = Sut.ConfirmUserDetails_Post();

        // Assert
        result.Should().BeOfType<RedirectToActionResult>();

        var redirectToActionResult = result as RedirectToActionResult;
        redirectToActionResult!.ControllerName.Should().BeNull();
        redirectToActionResult.ActionName.Should().Be("Index");
    }
}
