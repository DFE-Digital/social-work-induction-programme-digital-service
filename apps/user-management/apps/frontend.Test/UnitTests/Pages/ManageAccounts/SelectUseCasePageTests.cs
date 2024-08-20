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

public class SelectUseCasePageTests : ManageAccountsPageTestBase<SelectUseCase>
{
    private SelectUseCase Sut { get; }

    public SelectUseCasePageTests()
    {
        Sut = new SelectUseCase(
            CreateAccountJourneyService,
            new SelectUseCaseValidator(),
            new FakeLinkGenerator(),
            EditAccountJourneyService
        );
    }

    [Fact]
    public void Get_WhenCalled_LoadsTheView()
    {
        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();
        Sut.BackLinkPath.Should().Be("/manage-accounts/select-account-type");
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

    [Fact]
    public void GetEdit_WhenCalled_LoadsTheViewAndPopulatesModel()
    {
        // Arrange
        var account = AccountRepository.GetAll().PickRandom();

        // Act
        var result = Sut.OnGetEdit(account.Id);

        // Assert
        result.Should().BeOfType<PageResult>();
        Sut.BackLinkPath.Should()
            .Be($"/manage-accounts/select-account-type/{account.Id}?handler=Edit");
        Sut.EditAccountId.Should().Be(account.Id);
        Sut.SelectedAccountTypes.Should()
            .BeEquivalentTo(EditAccountJourneyService.GetAccountTypes(account.Id));
    }

    [Fact]
    public void GetEdit_WhenCalledWithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var result = Sut.OnGetEdit(invalidId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
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

    [Fact]
    public async Task PostEdit_WhenCalled_UpdatesAccountTypesAndRedirectsToViewAccountDetails()
    {
        // Arrange
        var account = AccountRepository.GetAll().PickRandom();
        var random = new Bogus.Randomizer();
        var updatedAccountTypes = random.ArrayElements(
            [AccountType.Assessor, AccountType.Coordinator],
            random.Number(1, 2)
        );

        Sut.SelectedAccountTypes = updatedAccountTypes;

        // Act
        var result = await Sut.OnPostEditAsync(account.Id);

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-accounts/view-account-details/" + account.Id);

        var updatedAccount = AccountRepository.GetById(account.Id);
        updatedAccount.Should().NotBeNull();
        updatedAccount!.Types.Should().BeEquivalentTo(updatedAccountTypes);
    }

    [Fact]
    public async Task PostEdit_WhenModelInvalid_AddsErrorsToModelState()
    {
        // Arrange
        var account = AccountRepository.GetAll().PickRandom();
        Sut.SelectedAccountTypes = new List<AccountType>();

        // Act
        var result = await Sut.OnPostEditAsync(account.Id);

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
        Sut.EditAccountId.Should().Be(account.Id);
        Sut.BackLinkPath.Should()
            .Be($"/manage-accounts/select-account-type/{account.Id}?handler=Edit");
    }

    [Fact]
    public async Task PostEdit_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var result = await Sut.OnPostEditAsync(invalidId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }
}
