using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageAccounts;

public class SelectAccountTypePageTests : ManageAccountsPageTestBase
{
    private SelectAccountType Sut { get; }

    public SelectAccountTypePageTests()
    {
        Sut = new SelectAccountType(CreateAccountJourneyService);
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
    public void Post_WhenCalled_RedirectsToAddUserDetails()
    {
        // Arrange
        Sut.SelectedAccountType = AccountType.Coordinator;

        // Act
        var result = Sut.OnPost();

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();

        var redirectToPageResult = result as RedirectToPageResult;
        redirectToPageResult!.PageName.Should().Be("AddAccountDetails");
    }

    [Fact]
    public void Post_WhenPassedAssessorCoordinator_RedirectsToSelectUseCase()
    {
        // Arrange
        Sut.SelectedAccountType = AccountType.AssessorCoordinator;

        // Act
        var result = Sut.OnPost();

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();

        var redirectToPageResult = result as RedirectToPageResult;
        redirectToPageResult!.PageName.Should().Be("SelectUseCase");
    }
}
