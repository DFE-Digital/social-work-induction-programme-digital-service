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

        MockEditAccountJourneyService.Verify(x => x.IsAccountIdValidAsync(account.Id), Times.Once);
        MockEditAccountJourneyService.Verify(x => x.GetAccountDetailsAsync(account.Id), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task Get_WhenCalledWithInvalidId_ReturnsNotFound()
    {
        var invalidId = Guid.NewGuid();

        MockEditAccountJourneyService
            .Setup(x => x.IsAccountIdValidAsync(invalidId))
            .ReturnsAsync(false);

        // Act
        var result = await Sut.OnGetAsync(invalidId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();

        MockEditAccountJourneyService.Verify(x => x.IsAccountIdValidAsync(invalidId), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task Post_WhenCalled_UpdatesAccountStatusAndRedirectsToAccountDetails()
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

        MockEditAccountJourneyService.Setup(x =>
            x.SetAccountStatusAsync(account.Id, AccountStatus.Inactive)
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
        MockEditAccountJourneyService.Verify(
            x => x.GetAccountDetailsAsync(account.Id),
            Times.Once()
        );
        MockEditAccountJourneyService.Verify(
            x => x.SetAccountStatusAsync(account.Id, AccountStatus.Inactive),
            Times.Once()
        );
        MockEditAccountJourneyService.Verify(x => x.CompleteJourneyAsync(account.Id), Times.Once());
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task Post_WhenCalledWithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        MockEditAccountJourneyService
            .Setup(x => x.IsAccountIdValidAsync(invalidId))
            .ReturnsAsync(false);

        // Act
        var result = await Sut.OnPostAsync(invalidId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();

        MockEditAccountJourneyService.Verify(x => x.IsAccountIdValidAsync(invalidId), Times.Once);
        VerifyAllNoOtherCalls();
    }
}
