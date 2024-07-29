using Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;
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
        Sut = new ViewAccountDetails(AccountRepository);
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

    [Fact]
    public void Get_WhenCalledWithNull_ReturnsNotFound()
    {
        // Act
        var result = Sut.OnGet(null);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }
}
