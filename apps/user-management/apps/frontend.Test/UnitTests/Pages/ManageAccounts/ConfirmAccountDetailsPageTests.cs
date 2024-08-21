using Dfe.Sww.Ecf.Frontend.Extensions;
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

public class ConfirmAccountDetailsShould : ManageAccountsPageTestBase<ConfirmAccountDetails>
{
    private ConfirmAccountDetails Sut { get; }

    public ConfirmAccountDetailsShould()
    {
        Sut = new ConfirmAccountDetails(
            CreateAccountJourneyService,
            EditAccountJourneyService,
            new FakeLinkGenerator()
        )
        {
            TempData = TempData
        };
    }

    [Fact]
    public void Get_WhenCalled_LoadsTheViewWithCorrectValues()
    {
        // Arrange
        var expectedAccountDetails = AccountDetails.FromAccount(AccountFaker.GenerateNewAccount());
        CreateAccountJourneyService.SetAccountDetails(expectedAccountDetails);

        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();

        Sut.FirstName.Should().Be(expectedAccountDetails.FirstName);
        Sut.LastName.Should().Be(expectedAccountDetails.LastName);
        Sut.Email.Should().Be(expectedAccountDetails.Email);
        Sut.SocialWorkEnglandNumber.Should().Be(expectedAccountDetails.SocialWorkEnglandNumber);
    }

    [Fact]
    public void Get_WhenCalled_HasCorrectBackLinkAndChangeDetailsLinkPaths()
    {
        // Arrange
        var expectedAccountDetails = AccountDetails.FromAccount(AccountFaker.GenerateNewAccount());
        CreateAccountJourneyService.SetAccountDetails(expectedAccountDetails);

        // Act
        _ = Sut.OnGet();

        // Assert
        Sut.IsUpdatingAccount.Should().BeFalse();
        Sut.BackLinkPath.Should().Be("/manage-accounts/add-account-details");
        Sut.ChangeDetailsLink.Should().Be("/manage-accounts/add-account-details?handler=Change");
    }

    [Fact]
    public void GetUpdate_WhenCalled_LoadsTheViewWithCorrectValues()
    {
        // Arrange
        var account = AccountRepository.GetAll().PickRandom();
        var updatedAccountDetails = AccountDetails.FromAccount(AccountFaker.GenerateNewAccount());
        EditAccountJourneyService.SetAccountDetails(account.Id, updatedAccountDetails);

        // Act
        var result = Sut.OnGetUpdate(account.Id);

        // Assert
        result.Should().BeOfType<PageResult>();

        Sut.Id.Should().Be(account.Id);
        Sut.FirstName.Should().Be(updatedAccountDetails.FirstName);
        Sut.LastName.Should().Be(updatedAccountDetails.LastName);
        Sut.Email.Should().Be(updatedAccountDetails.Email);
        Sut.SocialWorkEnglandNumber.Should().Be(updatedAccountDetails.SocialWorkEnglandNumber);
    }

    [Fact]
    public void GetUpdate_WhenCalled_HasCorrectBackLinkAndChangeDetailsLinkPaths()
    {
        // Arrange
        var accountId = AccountRepository.GetAll().PickRandom().Id;

        // Act
        _ = Sut.OnGetUpdate(accountId);

        // Assert
        Sut.IsUpdatingAccount.Should().BeTrue();
        Sut.BackLinkPath.Should().Be("/manage-accounts/edit-account-details/" + accountId);
        Sut.ChangeDetailsLink.Should()
            .Be("/manage-accounts/edit-account-details/" + accountId + "?handler=Change");
    }

    [Fact]
    public void GetUpdate_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var result = Sut.OnGetUpdate(invalidId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public void Post_WhenCalled_RedirectsToAccountsIndex()
    {
        // Arrange
        var account = AccountFaker.GenerateNewAccount();

        CreateAccountJourneyService.SetAccountTypes(account.Types!);
        CreateAccountJourneyService.SetAccountDetails(AccountDetails.FromAccount(account));

        // Act
        var result = Sut.OnPost();

        // Assert
        result.Should().BeOfType<RedirectResult>();

        result.Url.Should().Be("/manage-accounts");
    }

    [Fact]
    public void PostUpdate_WhenCalled_UpdatesAccountDetailsAndRedirectsToAccountsIndex()
    {
        // Arrange
        var account = AccountRepository.GetAll().PickRandom();
        var updatedAccountDetails = new AccountDetails()
        {
            FirstName = "UpdatedFirstName",
            LastName = "UpdatedLastName",
            Email = "UpdatedEmail",
            SocialWorkEnglandNumber = account.SocialWorkEnglandNumber
        };
        EditAccountJourneyService.SetAccountDetails(account.Id, updatedAccountDetails);

        // Act
        var result = Sut.OnPostUpdate(account.Id);

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-accounts/view-account-details/" + account.Id);
        AccountRepository.GetById(account.Id).Should().BeEquivalentTo(updatedAccountDetails);
    }

    [Fact]
    public void PostUpdate_WhenCalledWithInvalidId_ReturnsNotFound()
    {
        // Act
        var result = Sut.OnPostUpdate(Guid.NewGuid());

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }
}
