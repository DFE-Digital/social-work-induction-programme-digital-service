using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageAccounts;

public class PauseAccountPageTests : ManageAccountsPageTestBase<PauseAccount>
{
    private PauseAccount Sut { get; }

    public PauseAccountPageTests()
    {
        Sut = new PauseAccount(
            MockEditAccountJourneyService.Object,
            new FakeLinkGenerator(),
            MockEmailService.Object
        )
        {
            TempData = TempData,
            PageContext = new PageContext { HttpContext = HttpContext }
        };
    }

    [Fact]
    public async Task Get_WhenCalled_LoadsTheViewWithAccountDetails()
    {
        // Arrange
        var account = AccountFaker.Generate();
        var accountDetails = AccountDetails.FromAccount(account);

        MockEditAccountJourneyService
            .Setup(x => x.GetAccountDetailsAsync(account.Id))
            .ReturnsAsync(accountDetails);

        // Act
        var result = await Sut.OnGetAsync(account.Id);

        // Assert
        result.Should().BeOfType<PageResult>();

        Sut.Id.Should().Be(account.Id);
        Sut.AccountDetails.Should().Be(accountDetails);
        Sut.BackLinkPath.Should().Be("/manage-accounts/view-account-details/" + account.Id);

        MockEditAccountJourneyService.Verify(x => x.GetAccountDetailsAsync(account.Id), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task Get_WhenCalledWithInvalidId_ReturnsNotFound()
    {
        var invalidId = Guid.NewGuid();

        MockEditAccountJourneyService
            .Setup(x => x.GetAccountDetailsAsync(invalidId))
            .ReturnsAsync((AccountDetails?)null);

        // Act
        var result = await Sut.OnGetAsync(invalidId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();

        MockEditAccountJourneyService.Verify(x => x.GetAccountDetailsAsync(invalidId), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task Post_WhenCalled_UpdatesAccountStatusAndRedirectsToAccountDetails()
    {
        // Arrange
        var account = AccountFaker.Generate();
        var accountDetails = AccountDetails.FromAccount(account);

        MockEditAccountJourneyService
            .Setup(x => x.GetAccountDetailsAsync(account.Id))
            .ReturnsAsync(accountDetails);

        MockEditAccountJourneyService
            .Setup(x => x.GetAccountTypesAsync(account.Id))
            .ReturnsAsync(account.Types);

        MockEmailService
            .Setup(x =>
                x.PauseAccountAsync(
                    MoqHelpers.ShouldBeEquivalentTo(accountDetails),
                    MoqHelpers.ShouldBeEquivalentTo(account.Types),
                    UserConstants.UserName,
                    UserConstants.UserEmail
                )
            )
            .ReturnsAsync(true);

        MockEditAccountJourneyService.Setup(x =>
            x.SetAccountStatusAsync(account.Id, AccountStatus.Paused)
        );

        MockEditAccountJourneyService.Setup(x => x.CompleteJourneyAsync(account.Id));

        // Act
        var result = await Sut.OnPostAsync(account.Id);

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-accounts/view-account-details/" + account.Id);

        Sut.TempData["NotifyEmail"].Should().Be(account.Email);
        Sut.TempData["NotificationBannerSubject"].Should().Be("Account was successfully paused");

        MockEditAccountJourneyService.Verify(
            x => x.GetAccountDetailsAsync(account.Id),
            Times.Once()
        );
        MockEditAccountJourneyService.Verify(x => x.GetAccountTypesAsync(account.Id), Times.Once);
        MockEmailService.Verify(
            x =>
                x.PauseAccountAsync(
                    MoqHelpers.ShouldBeEquivalentTo(accountDetails),
                    MoqHelpers.ShouldBeEquivalentTo(account.Types),
                    UserConstants.UserName,
                    UserConstants.UserEmail
                ),
            Times.Once
        );
        MockEditAccountJourneyService.Verify(
            x => x.SetAccountStatusAsync(account.Id, AccountStatus.Paused),
            Times.Once
        );
        MockEditAccountJourneyService.Verify(x => x.CompleteJourneyAsync(account.Id), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task Post_WhenCalledWithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        MockEditAccountJourneyService
            .Setup(x => x.GetAccountDetailsAsync(invalidId))
            .ReturnsAsync((AccountDetails?)null);
        MockEditAccountJourneyService
            .Setup(x => x.GetAccountTypesAsync(invalidId))
            .ReturnsAsync((ImmutableList<AccountType>?)null);

        // Act
        var result = await Sut.OnPostAsync(invalidId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();

        MockEditAccountJourneyService.Verify(x => x.GetAccountDetailsAsync(invalidId), Times.Once);
        MockEditAccountJourneyService.Verify(x => x.GetAccountTypesAsync(invalidId), Times.Once);
        VerifyAllNoOtherCalls();
    }
}
