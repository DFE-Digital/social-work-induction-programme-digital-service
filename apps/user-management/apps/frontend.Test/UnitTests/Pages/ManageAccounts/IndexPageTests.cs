using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Pagination;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;
using ManageAccountsIndex = Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts.Index;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageAccounts;

public class IndexPageTests : ManageAccountsPageTestBase<ManageAccountsIndex>
{
    private ManageAccountsIndex Sut { get; }

    public IndexPageTests()
    {
        Sut = new ManageAccountsIndex(MockAccountService.Object);
    }

    [Fact]
    public async Task Get_WhenCalled_LoadsTheViewWithAccountsSortedByCreatedAt()
    {
        // Arrange
        var expectedAccounts = AccountFaker.Generate(10);

        var paginationRequest = new PaginationRequest(0, 10);
        var paginationResponse = new PaginationResult<Account>
        {
            Records = expectedAccounts,
            MetaData = new PaginationMetaData
            {
                Page = 1,
                PageSize = 5,
                PageCount = 2,
                TotalCount = 10,
                Links = new Dictionary<string, MetaDataLink>()
            }
        };

        MockAccountService
            .Setup(x => x.GetAllAsync(MoqHelpers.ShouldBeEquivalentTo(paginationRequest)))
            .ReturnsAsync(paginationResponse);

        // Act
        var result = await Sut.OnGetAsync();

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
        newAccount = await CreateAccountJourneyService.CompleteJourneyAsync();

        var paginationRequest = new PaginationRequest(0, 10);
        var paginationResponse = new PaginationResult<Account>
        {
            Records = new List<Account> { newAccount },
            MetaData = new PaginationMetaData
            {
                Page = 1,
                PageSize = 5,
                PageCount = 2,
                TotalCount = 10,
                Links = new Dictionary<string, MetaDataLink>()
            }
        };

        MockAccountService
            .Setup(x => x.GetAllAsync(MoqHelpers.ShouldBeEquivalentTo(paginationRequest)))
            .ReturnsAsync(paginationResponse);

        // Act
        await Sut.OnGetAsync();

        // Assert
        Sut.Accounts.Should().ContainEquivalentOf(newAccount);
    }
}
