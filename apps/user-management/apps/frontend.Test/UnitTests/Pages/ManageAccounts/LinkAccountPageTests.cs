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

public class LinkAccountPageTests : ManageAccountsPageTestBase<EditAccountDetails>
{
    private LinkAccount Sut { get; }

    public LinkAccountPageTests()
    {
        Sut = new LinkAccount(MockEditAccountJourneyService.Object, new FakeLinkGenerator())
        {
            TempData = TempData
        };
    }

    [Fact]
    public async Task Get_WhenCalled_LoadsTheViewWithAccountDetails()
    {
        // Arrange
        var account = AccountFaker.Generate();
        var accountDetails = AccountDetails.FromAccount(account);

        MockEditAccountJourneyService
            .Setup(x => x.IsAccountIdValidAsync(account.Id))
            .ReturnsAsync(true);
        MockEditAccountJourneyService
            .Setup(x => x.GetAccountDetailsAsync(account.Id))
            .ReturnsAsync(accountDetails);

        // Act
        var result = await Sut.OnGetAsync(account.Id);

        // Assert
        result.Should().BeOfType<PageResult>();

        Sut.Id.Should().Be(account.Id);
        Sut.Email.Should().Be(account.Email);
        Sut.BackLinkPath.Should().Be("/manage-accounts/view-account-details/" + account.Id);

        MockEditAccountJourneyService.Verify(
            x => x.IsAccountIdValidAsync(account.Id),
            Times.Once()
        );
        MockEditAccountJourneyService.Verify(
            x => x.GetAccountDetailsAsync(account.Id),
            Times.Once()
        );
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task Get_WhenCalledWithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();

        MockEditAccountJourneyService.Setup(x => x.IsAccountIdValidAsync(id)).ReturnsAsync(false);

        // Act
        var result = await Sut.OnGetAsync(id);

        // Assert
        result.Should().BeOfType<NotFoundResult>();

        MockEditAccountJourneyService.Verify(x => x.IsAccountIdValidAsync(id), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task Post_WhenCalledForSocialWorkerPendingRegistration_UpdatesAccountStatusAndRedirectsToAccountDetails()
    {
        // Arrange
        var account = new AccountFaker().GenerateSocialWorkerWithNoSweNumber();
        var accountDetails = AccountDetails.FromAccount(account);

        MockEditAccountJourneyService
            .Setup(x => x.IsAccountIdValidAsync(account.Id))
            .ReturnsAsync(true);
        MockEditAccountJourneyService
            .Setup(x => x.GetAccountDetailsAsync(account.Id))
            .ReturnsAsync(accountDetails);
        MockEditAccountJourneyService.Setup(x => x.GetIsStaffAsync(account.Id)).ReturnsAsync(false);
        MockEditAccountJourneyService.Setup(x =>
            x.SetAccountStatusAsync(account.Id, AccountStatus.PendingRegistration)
        );
        MockEditAccountJourneyService.Setup(x => x.CompleteJourneyAsync(account.Id));

        // Act
        var result = await Sut.OnPostAsync(account.Id);

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-accounts");

        Sut.TempData["NotifyEmail"].Should().Be(account.Email);

        MockEditAccountJourneyService.Verify(x => x.IsAccountIdValidAsync(account.Id), Times.Once);
        MockEditAccountJourneyService.Verify(x => x.GetAccountDetailsAsync(account.Id), Times.Once);
        MockEditAccountJourneyService.Setup(x => x.GetIsStaffAsync(account.Id)).ReturnsAsync(false);
        MockEditAccountJourneyService.Verify(
            x => x.SetAccountStatusAsync(account.Id, AccountStatus.PendingRegistration),
            Times.Once
        );
        MockEditAccountJourneyService.Verify(x => x.GetIsStaffAsync(account.Id), Times.Once);
        MockEditAccountJourneyService.Verify(x => x.CompleteJourneyAsync(account.Id), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task Post_WhenCalledForSocialWorkerWithSweNumber_UpdatesAccountStatusAndRedirectsToAccountDetails()
    {
        // Arrange
        var account = new AccountFaker().GenerateSocialWorker();
        var accountDetails = AccountDetails.FromAccount(account);

        MockEditAccountJourneyService
            .Setup(x => x.IsAccountIdValidAsync(account.Id))
            .ReturnsAsync(true);
        MockEditAccountJourneyService
            .Setup(x => x.GetAccountDetailsAsync(account.Id))
            .ReturnsAsync(accountDetails);
        MockEditAccountJourneyService.Setup(x =>
            x.SetAccountStatusAsync(account.Id, AccountStatus.Active)
        );
        MockEditAccountJourneyService.Setup(x => x.GetIsStaffAsync(account.Id)).ReturnsAsync(false);
        MockEditAccountJourneyService.Setup(x => x.CompleteJourneyAsync(account.Id));

        // Act
        var result = await Sut.OnPostAsync(account.Id);

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-accounts");

        Sut.TempData["NotifyEmail"].Should().Be(account.Email);

        MockEditAccountJourneyService.Verify(x => x.IsAccountIdValidAsync(account.Id), Times.Once);
        MockEditAccountJourneyService.Verify(x => x.GetAccountDetailsAsync(account.Id), Times.Once);
        MockEditAccountJourneyService.Verify(
            x => x.SetAccountStatusAsync(account.Id, AccountStatus.Active),
            Times.Once
        );
        MockEditAccountJourneyService.Verify(x => x.GetIsStaffAsync(account.Id), Times.Once);
        MockEditAccountJourneyService.Verify(x => x.CompleteJourneyAsync(account.Id), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task Post_WhenCalledWithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();

        MockEditAccountJourneyService.Setup(x => x.IsAccountIdValidAsync(id)).ReturnsAsync(false);

        // Act
        var result = await Sut.OnPostAsync(id);

        // Assert
        result.Should().BeOfType<NotFoundResult>();

        MockEditAccountJourneyService.Verify(x => x.IsAccountIdValidAsync(id), Times.Once);
    }
}
