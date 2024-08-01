using Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Extensions;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
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
    public void GetNew_WhenCalled_ResetsJourneyModelAndRedirectsToGetHandler()
    {
        // Arrange
        CreateAccountJourneyService.PopulateJourneyModelFromAccount(
            AccountFaker.GenerateNewAccount()
        );

        // Act
        var result = Sut.OnGetNew();

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        var redirectToPageResult = result as RedirectToPageResult;
        redirectToPageResult!.PageName.Should().Be(nameof(SelectAccountType));
        redirectToPageResult.PageHandler.Should().BeNull();

        CreateAccountJourneyService.GetAccountDetails().Should().BeNull();
        CreateAccountJourneyService.GetAccountTypes().Should().BeNull();
        CreateAccountJourneyService.GetIsStaff().Should().BeNull();
    }

    [Fact]
    public void Post_WhenIsStaffFalse_RedirectsToAddAccountDetails()
    {
        // Arrange
        Sut.IsStaff = false;

        // Act
        var result = Sut.OnPost();

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();

        var redirectToPageResult = result as RedirectToPageResult;
        redirectToPageResult!.PageName.Should().Be("AddAccountDetails");
    }

    [Fact]
    public void Post_WhenIsStaffTrue_RedirectsToSelectUseCase()
    {
        // Arrange
        Sut.IsStaff = true;

        // Act
        var result = Sut.OnPost();

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();

        var redirectToPageResult = result as RedirectToPageResult;
        redirectToPageResult!.PageName.Should().Be("SelectUseCase");
    }

    [Fact]
    public void Post_WhenModelInvalid_AddsErrorsToModelState()
    {
        // Arrange
        Sut.IsStaff = null;

        // Act
        Sut.ValidateModel();
        var result = Sut.OnPost();

        // Assert
        result.Should().BeOfType<PageResult>();
        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);
        modelStateKeys.Should().Contain("IsStaff");
        modelState["IsStaff"]!.Errors.Count.Should().Be(1);
        modelState["IsStaff"]!.Errors[0].ErrorMessage.Should().Be("Select who you want to add");
    }
}
