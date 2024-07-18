using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using Dfe.Sww.Ecf.Frontend.Views.Accounts;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Controllers.AccountsControllerTests;

public class IndexShould : AccountsControllerTestBase
{
    [Fact]
    public void Get_WhenCalled_LoadsTheView()
    {
        // Act
        var result = Sut.Index();

        // Assert
        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public void Get_WhenUserIsAdded_DisplayAddedUserInView()
    {
        // Arrange
        var newAccount = AccountFaker.GenerateNewUser();

        CreateAccountJourneyService.SetAccountType(new SelectUserTypeModel { AccountType = newAccount.Types!.First() });
        CreateAccountJourneyService.SetUserDetails(AddUserDetailsModel.FromAccount(newAccount));
        newAccount = CreateAccountJourneyService.CompleteJourney();

        // Act
        var result = Sut.Index();

        // Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = result as ViewResult;
        var viewModel = (IEnumerable<Account>)viewResult!.ViewData.Model!;
        viewModel.Should().ContainEquivalentOf(newAccount);
    }
}
