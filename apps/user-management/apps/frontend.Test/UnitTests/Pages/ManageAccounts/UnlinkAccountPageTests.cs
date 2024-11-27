using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Extensions;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Validation;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageAccounts;

public class UnlinkAccountPageTests : ManageAccountsPageTestBase<EditAccountDetails>
{
    private UnlinkAccount Sut { get; }

    public UnlinkAccountPageTests()
    {
        Sut = new UnlinkAccount(MockEditAccountJourneyService.Object, new FakeLinkGenerator())
        {
            TempData = TempData
        };
    }

    [Fact]
    public void Get_WhenCalled_LoadsTheViewWithAccountDetails()
    {
        // Arrange
        var account = AccountFaker.Generate();
        var accountDetails = AccountDetails.FromAccount(account);

        MockEditAccountJourneyService.Setup(x => x.IsAccountIdValid(account.Id)).Returns(true);
        MockEditAccountJourneyService
            .Setup(x => x.GetAccountDetails(account.Id))
            .Returns(accountDetails);

        // Act
        var result = Sut.OnGet(account.Id);

        // Assert
        result.Should().BeOfType<PageResult>();

        Sut.Id.Should().Be(account.Id);
        Sut.Email.Should().Be(account.Email);
        Sut.BackLinkPath.Should().Be("/manage-accounts/view-account-details/" + account.Id);

        MockEditAccountJourneyService.Verify(x => x.IsAccountIdValid(account.Id), Times.Once);
        MockEditAccountJourneyService.Verify(x => x.GetAccountDetails(account.Id), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public void Get_WhenCalledWithInvalidId_ReturnsNotFound()
    {
        var invalidId = Guid.NewGuid();

        MockEditAccountJourneyService.Setup(x => x.IsAccountIdValid(invalidId)).Returns(false);

        // Act
        var result = Sut.OnGet(invalidId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();

        MockEditAccountJourneyService.Verify(x => x.IsAccountIdValid(invalidId), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public void Post_WhenCalled_UpdatesAccountStatusAndRedirectsToAccountDetails()
    {
        // Arrange
        var account = AccountFaker.Generate();
        var accountDetails = AccountDetails.FromAccount(account);

        MockEditAccountJourneyService.Setup(x => x.IsAccountIdValid(account.Id)).Returns(true);
        MockEditAccountJourneyService
            .Setup(x => x.GetAccountDetails(account.Id))
            .Returns(accountDetails);

        MockEditAccountJourneyService.Setup(x =>
            x.SetAccountStatus(account.Id, AccountStatus.Inactive)
        );
        MockEditAccountJourneyService.Setup(x => x.CompleteJourney(account.Id));

        // Act
        var result = Sut.OnPost(account.Id);

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-accounts");

        Sut.TempData["NotifyEmail"].Should().Be(account.Email);

        MockEditAccountJourneyService.Verify(x => x.IsAccountIdValid(account.Id), Times.Once);
        MockEditAccountJourneyService.Verify(x => x.GetAccountDetails(account.Id), Times.Once());
        MockEditAccountJourneyService.Verify(
            x => x.SetAccountStatus(account.Id, AccountStatus.Inactive),
            Times.Once()
        );
        MockEditAccountJourneyService.Verify(x => x.CompleteJourney(account.Id), Times.Once());
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public void Post_WhenCalledWithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        MockEditAccountJourneyService.Setup(x => x.IsAccountIdValid(invalidId)).Returns(false);

        // Act
        var result = Sut.OnPost(invalidId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();

        MockEditAccountJourneyService.Verify(x => x.IsAccountIdValid(invalidId), Times.Once);
        VerifyAllNoOtherCalls();
    }
}
