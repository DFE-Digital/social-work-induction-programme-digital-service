using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Xunit;
using Index = Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts.Index;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageAccounts;

public class IndexPageTests : ManageAccountsPageTestBase
{
    private Index Sut { get; }

    public IndexPageTests()
    {
        Sut = new Index(AccountRepository);
    }

    [Fact]
    public void Get_WhenCalled_LoadsTheView()
    {
        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();
    }

    [Fact]
    public void Get_WhenAccountIsAdded_ModelShouldContainNewAccount()
    {
        // Arrange
        var newAccount = AccountFaker.GenerateNewAccount();

        CreateAccountJourneyService.SetAccountTypes(
            new List<AccountType> { newAccount.Types!.First() }
        );
        CreateAccountJourneyService.SetAccountDetails(AccountDetails.FromAccount(newAccount));
        newAccount = CreateAccountJourneyService.CompleteJourney();

        // Act
        Sut.OnGet();

        // Assert
        Sut.Accounts.Should().ContainEquivalentOf(newAccount);
    }
}
