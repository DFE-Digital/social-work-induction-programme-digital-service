using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Validation;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageAccounts;

public class SelectAccountTypePageTests : ManageAccountsPageTestBase<SelectAccountType>
{
    private SelectAccountType Sut { get; }

    public SelectAccountTypePageTests()
    {
        Sut = new SelectAccountType(
            MockCreateAccountJourneyService.Object,
            new FakeLinkGenerator(),
            new SelectAccountTypeValidator()
        );
    }

    [Fact]
    public void OnGet_WhenCalled_LoadsTheView()
    {
        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();

        Sut.IsStaff.Should().BeNull();
        Sut.EditAccountId.Should().BeNull();
        Sut.BackLinkPath.Should().Be("/manage-accounts");
        Sut.Handler.Should().Be("");
        Sut.FromChangeLink.Should().BeFalse();

        MockCreateAccountJourneyService.Verify(x => x.GetIsStaff(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public void OnGetNew_WhenCalled_ResetsModelAndRedirectsToSelectAccountType()
    {
        // Act
        var result = Sut.OnGetNew();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        result.Url.Should().Be("/manage-accounts/select-account-type");
        Sut.Handler.Should().Be("");
        Sut.FromChangeLink.Should().BeFalse();

        MockCreateAccountJourneyService.Verify(x => x.ResetCreateAccountJourneyModel(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithNullIsStaff_ReturnsErrorsAndRedirectsToSelectAccountType()
    {
        // Arrange
        Sut.IsStaff = null;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);
        modelStateKeys.Should().Contain("IsStaff");
        modelState["IsStaff"]!.Errors.Count.Should().Be(1);
        modelState["IsStaff"]!.Errors[0].ErrorMessage.Should().Be("Select the type of user you want to add");

        Sut.BackLinkPath.Should().Be("/manage-accounts");

        VerifyAllNoOtherCalls();
    }

    [Theory]
    [InlineData(true, "/manage-accounts/select-use-case?handler=Change")]
    [InlineData(false, "/manage-accounts/select-use-case")]
    public async Task OnPostAsync_WhenCalledWithIsStaffTrue_RedirectsToRelevantPageBasedOnFromChangeLink(
        bool fromChangeLink,
        string redirectPath
    )
    {
        // Arrange
        Sut.IsStaff = true;
        Sut.FromChangeLink = fromChangeLink;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be(redirectPath);

        MockCreateAccountJourneyService.Verify(x => x.SetIsStaff(true), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.GetAccountDetails(), Times.Once);

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithIsStaffFalse_RedirectsToEligibilityInformation()
    {
        // Arrange
        Sut.IsStaff = false;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-accounts/eligibility-information");

        MockCreateAccountJourneyService.Verify(x => x.SetIsStaff(false), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.SetAccountTypes(new List<AccountType>
            { AccountType.EarlyCareerSocialWorker }), Times.Once);

        MockCreateAccountJourneyService.Verify(x => x.GetAccountDetails(), Times.Once);

        VerifyAllNoOtherCalls();
    }

    [Theory]
    [InlineData(null, "/manage-accounts/add-account-details?handler=Change")]
    [InlineData("12345", "/manage-accounts/confirm-account-details")]
    public async Task OnPostAsync_WhenCalledFromChangeLinkAndIsStaffFalse_RedirectsToRelevantPage(
        string? socialWorkEnglandRegistrationNumber,
        string? redirectPath
    )
    {
        // Arrange
        Sut.IsStaff = false;
        Sut.FromChangeLink = true;
        var account = AccountBuilder
            .WithAddOrEditAccountDetailsData()
            .WithTypes(ImmutableList.Create(AccountType.EarlyCareerSocialWorker))
            .WithSocialWorkEnglandNumber(socialWorkEnglandRegistrationNumber)
            .Build();
        var accountDetails = AccountDetails.FromAccount(account);
        MockCreateAccountJourneyService.Setup(x => x.GetAccountDetails()).Returns(accountDetails);

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be(redirectPath);
        Sut.Handler.Should().Be("change");

        MockCreateAccountJourneyService.Verify(x => x.SetIsStaff(false), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.GetAccountDetails(), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.SetAccountTypes(new List<AccountType>
            { AccountType.EarlyCareerSocialWorker }), Times.Once);

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public void OnGetChange_WhenCalled_LoadsTheView()
    {
        // Arrange
        MockCreateAccountJourneyService.Setup(x => x.GetIsStaff()).Returns(false);

        // Act
        var result = Sut.OnGetChange();

        // Assert
        result.Should().BeOfType<PageResult>();

        Sut.BackLinkPath.Should().Be("/manage-accounts/confirm-account-details");
        Sut.Handler.Should().Be("change");
        Sut.FromChangeLink.Should().BeTrue();
        MockCreateAccountJourneyService.Verify(x => x.GetIsStaff(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostChangeAsync_WhenCalled_HasCorrectBackLink()
    {
        // Act
        _ = await Sut.OnPostChangeAsync();

        // Assert
        Sut.FromChangeLink.Should().BeTrue();
        Sut.BackLinkPath.Should().Be("/manage-accounts/confirm-account-details");
    }
}
