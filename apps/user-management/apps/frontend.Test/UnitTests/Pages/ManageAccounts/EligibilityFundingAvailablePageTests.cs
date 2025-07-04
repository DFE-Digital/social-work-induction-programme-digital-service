using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageAccounts;

public class EligibilityFundingAvailablePageTests : ManageAccountsPageTestBase<EligibilityFundingAvailable>
{
    private EligibilityFundingAvailable Sut { get; }

    public EligibilityFundingAvailablePageTests()
    {
        Sut = new EligibilityFundingAvailable(
            new FakeLinkGenerator(),
            MockCreateAccountJourneyService.Object
        );
    }

    [Fact]
    public void OnGet_WhenCalledAndSocialEnglandNumberNeedsCapturing_LoadsTheViewWithContinueToAccountDetailsPage()
    {
        // Arrange
        var account = AccountBuilder.WithSocialWorkEnglandNumber(null).Build();
        var accountDetails = AccountDetails.FromAccount(account);
        MockCreateAccountJourneyService.Setup(x => x.GetAccountDetails()).Returns(accountDetails);

        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();
        Sut.BackLinkPath.Should().Be("/manage-accounts/eligibility-qualification");
        Sut.NextPagePath.Should().Be("/manage-accounts/add-account-details");
        MockCreateAccountJourneyService.Verify(x => x.GetAccountDetails(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public void OnGet_WhenCalledAndProgrammeDatesNeedCapturing_LoadsTheViewWithContinueToProgrammeDatesPage()
    {
        //Arrange
        var account = AccountBuilder
            .WithTypes([AccountType.EarlyCareerSocialWorker])
            .WithSocialWorkEnglandNumber("12343").Build();
        var accountDetails = AccountDetails.FromAccount(account);
        MockCreateAccountJourneyService.Setup(x => x.GetAccountDetails()).Returns(accountDetails);
        MockCreateAccountJourneyService.Setup(x => x.GetProgrammeStartDate()).Returns((DateOnly?)null);

        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();
        Sut.BackLinkPath.Should().Be("/manage-accounts/eligibility-qualification");
        Sut.NextPagePath.Should().Be("/manage-accounts/social-worker-programme-dates");
        MockCreateAccountJourneyService.Verify(x => x.GetAccountDetails(), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.GetProgrammeStartDate(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public void OnGet_WhenCalledAndAllDetailsNeededAreAlreadyCaptured_LoadsTheViewWithContinueToConfirmDetailsPage()
    {
        //Arrange
        var account = AccountBuilder
            .WithTypes([AccountType.EarlyCareerSocialWorker])
            .WithSocialWorkEnglandNumber("12343").Build();
        var accountDetails = AccountDetails.FromAccount(account);
        MockCreateAccountJourneyService.Setup(x => x.GetAccountDetails()).Returns(accountDetails);
        MockCreateAccountJourneyService.Setup(x => x.GetProgrammeStartDate()).Returns(DateOnly.FromDateTime(DateTime.Now));

        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();
        Sut.BackLinkPath.Should().Be("/manage-accounts/eligibility-qualification");
        Sut.NextPagePath.Should().Be("/manage-accounts/confirm-account-details");
        MockCreateAccountJourneyService.Verify(x => x.GetAccountDetails(), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.GetProgrammeStartDate(), Times.Once);
        VerifyAllNoOtherCalls();
    }
}
