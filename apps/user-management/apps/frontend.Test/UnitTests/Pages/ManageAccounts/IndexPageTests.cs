using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Pagination;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
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
        var expectedAccounts = AccountBuilder.BuildMany(10);

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
    }
}
