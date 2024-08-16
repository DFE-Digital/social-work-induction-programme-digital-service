using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageAccounts;

public class ViewAccountDetailsPageTests : ManageAccountsPageTestBase
{
    private ViewAccountDetails Sut { get; }

    public ViewAccountDetailsPageTests()
    {
        Sut = new ViewAccountDetails(AccountRepository, new FakeLinkGenerator())
        {
            TempData = TempData
        };
    }

    [Fact]
    public void Get_WhenCalledWithId_LoadsTheView()
    {
        // Arrange
        var account = AccountRepository.GetAll().First();

        // Act
        var result = Sut.OnGet(account.Id);

        // Assert
        result.Should().BeOfType<PageResult>();
        Sut.Account.Should().BeEquivalentTo(account);
    }

    [Fact]
    public void Get_WhenCalledWithInvalidId_ReturnsNotFound()
    {
        // Act
        var result = Sut.OnGet(Guid.NewGuid());

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Get_Always_ClearsUpdatedAccountDetails(bool isAccountIdValid)
    {
        // Arrange
        var accountId = isAccountIdValid ? AccountRepository.GetAll().First().Id : Guid.NewGuid();
        TempData.Set("UpdatedAccountDetails-" + accountId, "foo");

        // Act
        _ = Sut.OnGet(accountId);

        // Assert
        TempData.Get<string>("UpdatedAccountDetails-" + accountId).Should().BeNull();
    }
}
