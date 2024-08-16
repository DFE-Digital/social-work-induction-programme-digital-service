using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Extensions;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
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
        Sut = new SelectUseCase(
            CreateAccountJourneyService,
            new SelectUseCaseValidator(),
            new FakeLinkGenerator()
        );
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
    public void Get_WhenCalled_PopulatesModelFromJourneyState()
    {
        // Arrange
        var account = AccountFaker.GenerateNewAccount();
        CreateAccountJourneyService.PopulateJourneyModelFromAccount(account);

        // Act
        _ = Sut.OnGet();

        // Assert
        Sut.SelectedAccountTypes.Should()
            .BeEquivalentTo(CreateAccountJourneyService.GetAccountTypes());
    }

    [Theory]
    [InlineData(AccountType.Coordinator)]
    [InlineData(AccountType.Coordinator, AccountType.Assessor)]
    public async Task Post_WhenCalledWithValidSelection_RedirectsToAddAccountDetails(
        params AccountType[] accountTypes
    )
    {
        // Arrange
        Sut.SelectedAccountTypes = accountTypes;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-accounts/add-account-details");
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
