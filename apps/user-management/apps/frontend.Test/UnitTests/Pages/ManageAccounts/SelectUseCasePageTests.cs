using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;
using Dfe.Sww.Ecf.Frontend.Validation;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageAccounts;

public class SelectUseCasePageTests : ManageAccountsPageTestBase
{
    private SelectUseCase Sut { get; }

    public SelectUseCasePageTests()
    {
        Sut = new SelectUseCase(CreateAccountJourneyService, new SelectUseCaseValidator());
    }

    [Fact]
    public void Get_WhenCalled_LoadsTheView()
    {
        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();
    }

    [Theory]
    [InlineData(AccountType.Coordinator)]
    [InlineData(AccountType.Coordinator, AccountType.Assessor)]
    public async Task Post_WhenCalledWithValidSelection_RedirectsToAddUserDetails(
        params AccountType[] accountTypes
    )
    {
        // Arrange
        Sut.SelectedAccountTypes = accountTypes;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();

        var redirectToPageResult = result as RedirectToPageResult;
        redirectToPageResult!.PageName.Should().Be(nameof(AddAccountDetails));
    }

    [Fact]
    public async Task Post_WhenSelectedAccountTypesIsEmpty_AddsErrorsToModelState()
    {
        // Arrange
        Sut.SelectedAccountTypes = new List<AccountType>();

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);
        modelStateKeys.Should().Contain("SelectedAccountTypes");
        modelState["SelectedAccountTypes"]!.Errors.Count.Should().Be(1);
        modelState["SelectedAccountTypes"]!
            .Errors[0]
            .ErrorMessage.Should()
            .Be("Select what they need to do");
    }
}
