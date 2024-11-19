using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Extensions;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageAccounts;

public class SelectAccountTypePageTests : ManageAccountsPageTestBase<SelectAccountType>
{
    private SelectAccountType Sut { get; }

    public SelectAccountTypePageTests()
    {
        Sut = new SelectAccountType(
            CreateAccountJourneyService,
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
        Sut.BackLinkPath.Should().Be("/manage-accounts");
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
        Sut.IsStaff.Should().Be(CreateAccountJourneyService.GetIsStaff());
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
        result.Should().BeOfType<RedirectResult>();
        result.Url.Should().Be("/manage-accounts/select-account-type");

        CreateAccountJourneyService.GetAccountDetails().Should().BeNull();
        CreateAccountJourneyService.GetAccountTypes().Should().BeNull();
        CreateAccountJourneyService.GetIsStaff().Should().BeNull();
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
        Sut.EditAccountId.Should().Be(account.Id);
        Sut.IsStaff.Should().BeNull();
        Sut.BackLinkPath.Should().Be("/manage-accounts/view-account-details/" + account.Id);
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

    [Fact]
    public void Post_WhenIsStaffFalse_RedirectsToAddAccountDetails()
    {
        // Arrange
        Sut.IsStaff = false;

        // Act
        var result = Sut.OnPost();

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-accounts/add-account-details");
    }

    [Fact]
    public void Post_WhenIsStaffTrue_RedirectsToSelectUseCase()
    {
        // Arrange
        Sut.IsStaff = true;

        // Act
        var result = Sut.OnPost();

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-accounts/select-use-case");
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
        Sut.BackLinkPath.Should().Be("/manage-accounts");
    }

    [Fact]
    public void PostEdit_WhenIsStaffFalse_UpdatesAccountTypeAndRedirectsToViewAccountDetails()
    {
        // Arrange
        var account = AccountRepository.GetAll().PickRandom();
        Sut.IsStaff = false;

        // Act
        var result = Sut.OnPostEdit(account.Id);

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-accounts/view-account-details/" + account.Id);

        var updatedAccount = AccountRepository.GetById(account.Id);
        updatedAccount.Should().NotBeNull();
        updatedAccount!.Types.Should().BeEquivalentTo([AccountType.EarlyCareerSocialWorker]);
        updatedAccount!.IsStaff.Should().BeFalse();
    }

    [Fact]
    public void PostEdit_WhenIsStaffTrue_RedirectsToEditUseCase()
    {
        // Arrange
        var account = AccountRepository.GetAll().PickRandom();
        Sut.IsStaff = true;

        // Act
        var result = Sut.OnPostEdit(account.Id);

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!
            .Url.Should()
            .Be("/manage-accounts/select-use-case/" + account.Id + "?handler=Edit");
    }

    [Fact]
    public void PostEdit_WhenModelInvalid_AddsErrorsToModelState()
    {
        // Arrange
        var account = AccountRepository.GetAll().PickRandom();
        Sut.IsStaff = null;

        // Act
        Sut.ValidateModel();
        var result = Sut.OnPostEdit(account.Id);

        // Assert
        result.Should().BeOfType<PageResult>();
        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);
        modelStateKeys.Should().Contain("IsStaff");
        modelState["IsStaff"]!.Errors.Count.Should().Be(1);
        modelState["IsStaff"]!.Errors[0].ErrorMessage.Should().Be("Select who you want to add");
        Sut.BackLinkPath.Should().Be("/manage-accounts/view-account-details/" + account.Id);
    }

    [Fact]
    public void PostEdit_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var result = Sut.OnPostEdit(invalidId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }
}
