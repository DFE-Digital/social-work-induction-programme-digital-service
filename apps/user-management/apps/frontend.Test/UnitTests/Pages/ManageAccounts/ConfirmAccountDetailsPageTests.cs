using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageAccounts;

public class ConfirmAccountDetailsShould : ManageAccountsPageTestBase
{
    private ConfirmAccountDetails Sut { get; }

    public ConfirmAccountDetailsShould()
    {
        Sut = new ConfirmAccountDetails(CreateAccountJourneyService) { TempData = TempData };
    }

    [Fact]
    public void Get_WhenCalled_LoadsTheView()
    {
        // Arrange
        var expectedAccountDetails = AccountDetails.FromAccount(AccountFaker.GenerateNewAccount());
        CreateAccountJourneyService.SetAccountDetails(expectedAccountDetails);

        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();

        Sut.FirstName.Should().Be(expectedAccountDetails.FirstName);
        Sut.LastName.Should().Be(expectedAccountDetails.LastName);
        Sut.Email.Should().Be(expectedAccountDetails.Email);
        Sut.SocialWorkEnglandNumber.Should().Be(expectedAccountDetails.SocialWorkEnglandNumber);
    }

    [Fact]
    public void Post_WhenCalled_RedirectsToAccountsIndex()
    {
        // Arrange
        var account = AccountFaker.GenerateNewAccount();

        CreateAccountJourneyService.SetAccountTypes(account.Types!);
        CreateAccountJourneyService.SetAccountDetails(AccountDetails.FromAccount(account));

        // Act
        var result = Sut.OnPost();

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();

        result.PageName.Should().Be("Index");
    }
}
