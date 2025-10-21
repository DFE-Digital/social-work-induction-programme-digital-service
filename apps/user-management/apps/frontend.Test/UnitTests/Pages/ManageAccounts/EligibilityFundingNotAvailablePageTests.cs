using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageAccounts;

public class EligibilityFundingNotAvailablePageTests : ManageAccountsPageTestBase<EligibilityFundingNotAvailable>
{
    private EligibilityFundingNotAvailable Sut { get; }

    public EligibilityFundingNotAvailablePageTests()
    {
        Sut = new EligibilityFundingNotAvailable(
            MockCreateAccountJourneyService.Object,
            new FakeLinkGenerator()
        );
    }

    [Theory]
    [InlineData(true, "/manage-accounts/eligibility-agency-worker")]
    [InlineData(false, "/manage-accounts/eligibility-qualification")]
    public void OnGet_WhenCalled_LoadsTheViewWithCorrectBackLinks(bool isAgencyWorker, string expectedBackLink)
    {
        var accountType = AccountType.EarlyCareerSocialWorker;

        MockCreateAccountJourneyService.Setup(x => x.GetIsAgencyWorker()).Returns(isAgencyWorker);
        MockCreateAccountJourneyService.Setup(x => x.GetAccountTypes()).Returns([accountType]);

        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();
        Sut.BackLinkPath.Should().Be(expectedBackLink);

        MockCreateAccountJourneyService.Verify(x => x.GetIsAgencyWorker(), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.GetAccountTypes(), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.GetAccountDetails(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public void OnGet_WhenCalledAndSocialEnglandNumberNeedsCapturing_LoadsTheViewWithContinueToAccountDetailsPage()
    {
        // Arrange
        var accountType = AccountType.EarlyCareerSocialWorker;
        var account = AccountBuilder.WithSocialWorkEnglandNumber(null).Build();
        var accountDetails = AccountDetails.FromAccount(account);
        MockCreateAccountJourneyService.Setup(x => x.GetAccountDetails()).Returns(accountDetails);
        MockCreateAccountJourneyService.Setup(x => x.GetAccountTypes()).Returns([accountType]);

        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();
        Sut.NextPagePath.Should().Be("/manage-accounts/add-account-details");
        MockCreateAccountJourneyService.Verify(x => x.GetIsAgencyWorker(), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.GetAccountDetails(), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.GetAccountTypes(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public void OnGet_WhenCalledAndProgrammeDatesNeedCapturing_LoadsTheViewWithContinueToProgrammeDatesPage()
    {
        //Arrange
        var accountType = AccountType.EarlyCareerSocialWorker;
        var account = AccountBuilder
            .WithTypes([accountType])
            .WithSocialWorkEnglandNumber("12343").Build();
        var accountDetails = AccountDetails.FromAccount(account);
        MockCreateAccountJourneyService.Setup(x => x.GetAccountDetails()).Returns(accountDetails);
        MockCreateAccountJourneyService.Setup(x => x.GetAccountTypes()).Returns([accountType]);
        MockCreateAccountJourneyService.Setup(x => x.GetProgrammeStartDate()).Returns((DateOnly?)null);

        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();
        Sut.NextPagePath.Should().Be("/manage-accounts/social-worker-programme-dates");
        MockCreateAccountJourneyService.Verify(x => x.GetIsAgencyWorker(), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.GetAccountDetails(), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.GetAccountTypes(), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.GetProgrammeStartDate(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public void OnGet_WhenCalledAndAllDetailsNeededAreAlreadyCaptured_LoadsTheViewWithContinueToConfirmDetailsPage()
    {
        //Arrange
        var accountType = AccountType.EarlyCareerSocialWorker;
        var account = AccountBuilder
            .WithTypes([accountType])
            .WithSocialWorkEnglandNumber("12343").Build();
        var accountDetails = AccountDetails.FromAccount(account);
        MockCreateAccountJourneyService.Setup(x => x.GetAccountDetails()).Returns(accountDetails);
        MockCreateAccountJourneyService.Setup(x => x.GetAccountTypes()).Returns([accountType]);
        MockCreateAccountJourneyService.Setup(x => x.GetProgrammeStartDate()).Returns(DateOnly.FromDateTime(DateTime.Now));

        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();
        Sut.NextPagePath.Should().Be("/manage-accounts/confirm-account-details");
        MockCreateAccountJourneyService.Verify(x => x.GetIsAgencyWorker(), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.GetAccountTypes(), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.GetAccountDetails(), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.GetProgrammeStartDate(), Times.Once);
        VerifyAllNoOtherCalls();
    }
}
