using Dfe.Sww.Ecf.Frontend.HttpClients.AccountsService.Models;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;
using ManageAccoutsIndex = Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts.Index;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageAccounts;

public class IndexPageTests : ManageAccountsPageTestBase<ManageAccoutsIndex>
{
    private ManageAccoutsIndex Sut { get; }

    public IndexPageTests()
    {
        Sut = new ManageAccoutsIndex(MockAccountService.Object);
    }

    [Fact]
    public async Task Get_WhenCalled_LoadsTheViewWithAccountsSortedByCreatedAt()
    {
        // Arrange
        var expectedAccounts = AccountFaker.Generate(10);
        MockAccountService.Setup(x => x.GetAllAsync()).ReturnsAsync(expectedAccounts);

        // Act
        var result = await Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();
        Sut.Accounts.Should().NotBeEmpty();
        Sut.Accounts.Should().BeEquivalentTo(expectedAccounts);
        Sut.Accounts.Should().BeInAscendingOrder(x => x.CreatedAt);
    }

    [Fact]
    public async Task Get_WhenAccountIsAdded_ModelShouldContainNewAccount()
    {
        // Arrange
        var newAccount = AccountFaker.GenerateNewAccount();

        CreateAccountJourneyService.SetAccountTypes(
            new List<AccountType> { newAccount.Types!.First() }
        );
        CreateAccountJourneyService.SetAccountDetails(AccountDetails.FromAccount(newAccount));
        newAccount = CreateAccountJourneyService.CompleteJourney();

        MockAccountService.Setup(x => x.GetAllAsync()).ReturnsAsync([newAccount]);

        // Act
        await Sut.OnGet();

        // Assert
        Sut.Accounts.Should().ContainEquivalentOf(newAccount);
    }
}
