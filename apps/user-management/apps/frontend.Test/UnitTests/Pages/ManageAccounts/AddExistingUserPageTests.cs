using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Models;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.NameMatch;
using Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using Dfe.Sww.Ecf.Frontend.Validation;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageAccounts;

public class AddExistingUserPageTests : ManageAccountsPageTestBase<AddExistingUser>
{
    private AddExistingUser Sut { get; }

    public AddExistingUserPageTests()
    {
        Sut = new AddExistingUser(
            MockCreateAccountJourneyService.Object,
            new FakeLinkGenerator(),
            MockSocialWorkEnglandService.Object
        )
        {
            TempData = TempData
        };
    }

    [Fact]
    public void OnGet_WhenCalled_LoadsTheView()
    {
        // Arrange
        var account = AccountFaker.Generate();
        var accountDetails = AccountDetails.FromAccount(account);

        var socialWorker = SocialWorkerFaker.Generate();
        var nameMatchScore = MatchResult.Good;

        MockCreateAccountJourneyService.Setup(x => x.GetAccountDetails()).Returns(accountDetails);
        MockCreateAccountJourneyService
            .Setup(x => x.GetSocialWorkerDetails())
            .Returns(socialWorker);
        MockSocialWorkEnglandService
            .Setup(x =>
                x.GetNameMatchScore(
                    accountDetails.FirstName,
                    account.LastName,
                    socialWorker.RegisteredName
                )
            )
            .Returns(nameMatchScore);

        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();

        // Assert
        Sut.Name.Should().Be(socialWorker.RegisteredName);
        Sut.SocialWorkEnglandNumber.Should().Be(socialWorker.Id);
        Sut.Email.Should().Be(account.Email);
        Sut.NameMatchResult.Should().Be(nameMatchScore);
        Sut.BackLinkPath.Should().Be("/manage-accounts/add-account-details");

        MockCreateAccountJourneyService.Verify(x => x.GetAccountDetails(), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.GetSocialWorkerDetails(), Times.Once);
        MockSocialWorkEnglandService.Verify(
            x =>
                x.GetNameMatchScore(
                    accountDetails.FirstName,
                    account.LastName,
                    socialWorker.RegisteredName
                ),
            Times.Once
        );
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public void OnGet_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        MockCreateAccountJourneyService
            .Setup(x => x.GetAccountDetails())
            .Returns((AccountDetails?)null);
        MockCreateAccountJourneyService
            .Setup(x => x.GetSocialWorkerDetails())
            .Returns((SocialWorker?)null);

        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();

        MockCreateAccountJourneyService.Verify(x => x.GetAccountDetails(), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.GetSocialWorkerDetails(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalled_RediectsToManageAccounts()
    {
        // Arrange
        var account = AccountFaker.Generate();
        var accountDetails = AccountDetails.FromAccount(account);

        MockCreateAccountJourneyService.Setup(x => x.GetAccountDetails()).Returns(accountDetails);
        MockCreateAccountJourneyService.Setup(x => x.CompleteJourneyAsync());

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();

        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-accounts");

        Sut.TempData["NotifyEmail"].Should().Be(account.Email);
        Sut.TempData["NotificationBannerSubject"].Should().Be("Account was successfully added");

        MockCreateAccountJourneyService.Verify(x => x.GetAccountDetails(), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.CompleteJourneyAsync(), Times.Once);
        VerifyAllNoOtherCalls();
    }
}
