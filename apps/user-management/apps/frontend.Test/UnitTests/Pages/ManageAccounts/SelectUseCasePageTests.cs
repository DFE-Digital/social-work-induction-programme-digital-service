using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Extensions;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using Dfe.Sww.Ecf.Frontend.Validation;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageAccounts;

public class SelectUseCasePageTests : ManageAccountsPageTestBase<SelectUseCase>
{
    private SelectUseCase Sut { get; }

    public SelectUseCasePageTests()
    {
        Sut = new SelectUseCase(
            MockCreateAccountJourneyService.Object,
            new SelectUseCaseValidator(),
            new FakeLinkGenerator(),
            MockEditAccountJourneyService.Object
        );
    }

    [Fact]
    public void Get_WhenCalled_LoadsTheView()
    {
        // Arrange
        var accountTypes = new List<AccountType>
        {
            AccountType.Coordinator,
            AccountType.Assessor,
            AccountType.EarlyCareerSocialWorker
        };

        MockCreateAccountJourneyService.Setup(x => x.GetAccountTypes()).Returns(accountTypes);

        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();
        Sut.BackLinkPath.Should().Be("/manage-accounts/select-account-type");
        Sut.SelectedAccountTypes.Should().BeEquivalentTo(accountTypes);

        MockCreateAccountJourneyService.Verify(x => x.GetAccountTypes(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public void GetEdit_WhenCalled_LoadsTheViewAndPopulatesModel()
    {
        // Arrange
        var account = AccountFaker.Generate();

        MockEditAccountJourneyService.Setup(x => x.IsAccountIdValid(account.Id)).Returns(true);

        // Act
        var result = Sut.OnGetEdit(account.Id);

        // Assert
        result.Should().BeOfType<PageResult>();
        Sut.BackLinkPath.Should()
            .Be($"/manage-accounts/select-account-type/{account.Id}?handler=Edit");
        Sut.EditAccountId.Should().Be(account.Id);
        Sut.SelectedAccountTypes.Should().BeNull();

        MockEditAccountJourneyService.Verify(x => x.IsAccountIdValid(account.Id), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public void GetEdit_WhenCalledWithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        MockEditAccountJourneyService.Setup(x => x.IsAccountIdValid(invalidId)).Returns(false);

        // Act
        var result = Sut.OnGetEdit(invalidId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();

        MockEditAccountJourneyService.Verify(x => x.IsAccountIdValid(invalidId), Times.Once);
        VerifyAllNoOtherCalls();
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

        MockCreateAccountJourneyService.Setup(x =>
            x.SetAccountTypes(MoqHelpers.ShouldBeEquivalentTo(accountTypes))
        );

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-accounts/add-account-details");

        MockCreateAccountJourneyService.Verify(
            x => x.SetAccountTypes(MoqHelpers.ShouldBeEquivalentTo(accountTypes)),
            Times.Once
        );
        VerifyAllNoOtherCalls();
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

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task PostEdit_WhenCalled_UpdatesAccountTypesAndRedirectsToViewAccountDetails()
    {
        // Arrange
        var account = AccountFaker.Generate();
        var accountDetails = AccountDetails.FromAccount(account);
        var random = new Bogus.Randomizer();
        var updatedAccountTypes = random.ArrayElements(
            [AccountType.Assessor, AccountType.Coordinator],
            random.Number(1, 2)
        );

        Sut.SelectedAccountTypes = updatedAccountTypes;

        MockEditAccountJourneyService.Setup(x => x.IsAccountIdValid(account.Id)).Returns(true);
        MockEditAccountJourneyService.Setup(x =>
            x.SetAccountTypes(account.Id, MoqHelpers.ShouldBeEquivalentTo(updatedAccountTypes))
        );
        MockEditAccountJourneyService.Setup(x => x.CompleteJourney(account.Id));

        // Act
        var result = await Sut.OnPostEditAsync(account.Id);

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-accounts/view-account-details/" + account.Id);

        MockEditAccountJourneyService.Verify(x => x.IsAccountIdValid(account.Id), Times.Once);
        MockEditAccountJourneyService.Verify(
            x =>
                x.SetAccountTypes(account.Id, MoqHelpers.ShouldBeEquivalentTo(updatedAccountTypes)),
            Times.Once
        );
        MockEditAccountJourneyService.Verify(x => x.CompleteJourney(account.Id), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task PostEdit_WhenModelInvalid_AddsErrorsToModelState()
    {
        // Arrange
        var account = AccountFaker.Generate();

        Sut.SelectedAccountTypes = new List<AccountType>();

        MockEditAccountJourneyService.Setup(x => x.IsAccountIdValid(account.Id)).Returns(true);

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

        MockEditAccountJourneyService.Verify(x => x.IsAccountIdValid(account.Id), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task PostEdit_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        MockEditAccountJourneyService.Setup(x => x.IsAccountIdValid(invalidId)).Returns(false);

        // Act
        var result = await Sut.OnPostEditAsync(invalidId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();

        MockEditAccountJourneyService.Verify(x => x.IsAccountIdValid(invalidId), Times.Once);
    }
}
