using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageAccounts;

public class ConfirmAccountDetailsShould : ManageAccountsPageTestBase<ConfirmAccountDetails>
{
    private ConfirmAccountDetails Sut { get; }

    public ConfirmAccountDetailsShould()
    {
        Sut = new ConfirmAccountDetails(
            MockCreateAccountJourneyService.Object,
            MockEditAccountJourneyService.Object,
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

        MockCreateAccountJourneyService
            .Setup(x => x.GetAccountDetails())
            .Returns(expectedAccountDetails);

        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();

        Sut.FirstName.Should().Be(expectedAccountDetails.FirstName);
        Sut.LastName.Should().Be(expectedAccountDetails.LastName);
        Sut.Email.Should().Be(expectedAccountDetails.Email);
        Sut.SocialWorkEnglandNumber.Should().Be(expectedAccountDetails.SocialWorkEnglandNumber);

        Sut.IsUpdatingAccount.Should().BeFalse();
        Sut.BackLinkPath.Should().Be("/manage-accounts/add-account-details");
        Sut.ChangeDetailsLink.Should().Be("/manage-accounts/add-account-details?handler=Change");

        MockCreateAccountJourneyService.Verify(x => x.GetAccountDetails(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public void GetUpdate_WhenCalled_LoadsTheViewWithCorrectValues()
    {
        // Arrange
        var account = AccountFaker.Generate();
        var updatedAccountDetails = AccountDetails.FromAccount(AccountFaker.GenerateNewAccount());

        MockEditAccountJourneyService.Setup(x => x.IsAccountIdValid(account.Id)).Returns(true);
        MockEditAccountJourneyService
            .Setup(x => x.GetAccountDetails(account.Id))
            .Returns(updatedAccountDetails);

        // Act
        var result = Sut.OnGetUpdate(account.Id);

        // Assert
        result.Should().BeOfType<PageResult>();

        Sut.Id.Should().Be(account.Id);
        Sut.FirstName.Should().Be(updatedAccountDetails.FirstName);
        Sut.LastName.Should().Be(updatedAccountDetails.LastName);
        Sut.Email.Should().Be(updatedAccountDetails.Email);
        Sut.SocialWorkEnglandNumber.Should().Be(updatedAccountDetails.SocialWorkEnglandNumber);

        Sut.IsUpdatingAccount.Should().BeTrue();
        Sut.BackLinkPath.Should().Be("/manage-accounts/edit-account-details/" + account.Id);
        Sut.ChangeDetailsLink.Should()
            .Be("/manage-accounts/edit-account-details/" + account.Id + "?handler=Change");

        MockEditAccountJourneyService.Verify(x => x.IsAccountIdValid(account.Id), Times.Once);
        MockEditAccountJourneyService.Verify(x => x.GetAccountDetails(account.Id), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task Post_WhenCalled_CreatesAccountAndSendsEmailToNewAccountWithInvitationTokenLink()
    {
        // Arrange
        var account = AccountFaker.Generate();
        var updatedAccountDetails = AccountDetails.FromAccount(account);

        MockCreateAccountJourneyService
            .Setup(x => x.GetAccountDetails())
            .Returns(updatedAccountDetails);

        MockCreateAccountJourneyService.Setup(x => x.CompleteJourneyAsync());

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Url.Should().Be("/manage-accounts");

        MockCreateAccountJourneyService.Verify(x => x.GetAccountDetails(), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.CompleteJourneyAsync(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public void GetUpdate_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        MockEditAccountJourneyService.Setup(x => x.IsAccountIdValid(invalidId)).Returns(false);

        // Act
        var result = Sut.OnGetUpdate(invalidId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();

        MockEditAccountJourneyService.Verify(x => x.IsAccountIdValid(invalidId), Times.Once());
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public void PostUpdate_WhenCalled_UpdatesAccountDetailsAndRedirectsToAccountsIndex()
    {
        // Arrange
        var account = AccountFaker.Generate();

        MockEditAccountJourneyService.Setup(x => x.IsAccountIdValid(account.Id)).Returns(true);
        MockEditAccountJourneyService.Setup(x => x.CompleteJourney(account.Id));

        // Act
        var result = Sut.OnPostUpdate(account.Id);

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-accounts/view-account-details/" + account.Id);

        MockEditAccountJourneyService.Verify(x => x.IsAccountIdValid(account.Id), Times.Once);
        MockEditAccountJourneyService.Verify(x => x.CompleteJourney(account.Id), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public void PostUpdate_WhenCalledWithInvalidId_ReturnsNotFound()
    {
        var id = Guid.NewGuid();
        MockEditAccountJourneyService.Setup(x => x.IsAccountIdValid(id)).Returns(false);

        // Act
        var result = Sut.OnPostUpdate(id);

        // Assert
        result.Should().BeOfType<NotFoundResult>();

        MockEditAccountJourneyService.Verify(x => x.IsAccountIdValid(id), Times.Once);
        VerifyAllNoOtherCalls();
    }
}
