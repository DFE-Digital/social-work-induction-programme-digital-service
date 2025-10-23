using Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Validation;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageAccounts;

public class EligibilityQualificationPageTests : ManageAccountsPageTestBase<EligibilityQualification>
{
    private EligibilityQualification Sut { get; }

    public EligibilityQualificationPageTests()
    {
        Sut = new EligibilityQualification(
            MockCreateAccountJourneyService.Object,
            new FakeLinkGenerator(),
            new EligibilityQualificationValidator()
        );
    }

    [Fact]
    public void OnGet_WhenCalled_LoadsTheView()
    {
        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();

        Sut.BackLinkPath.Should().Be("/manage-accounts/eligibility-agency-worker");
        MockCreateAccountJourneyService.Verify(x => x.GetIsRecentlyQualified(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void OnGet_WhenCalledWithIsRecentlyQualified_LoadsTheViewWithPrepopulatedValue(bool? isRecentlyQualified)
    {
        // Arrange
        MockCreateAccountJourneyService.Setup(x => x.GetIsRecentlyQualified())
            .Returns(isRecentlyQualified);

        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();
        Sut.IsRecentlyQualified.Should().Be(isRecentlyQualified);

        MockCreateAccountJourneyService.Verify(x => x.GetIsRecentlyQualified(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task
        OnPostAsync_WhenCalledWithNullIsRecentlyQualified_ReturnsErrorsAndRedirectsToEligibilityAgencyWorker(bool isFromChangeLink)
    {
        // Arrange
        Sut.FromChangeLink = isFromChangeLink;
        Sut.IsRecentlyQualified = null;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);
        modelStateKeys.Should().Contain("IsRecentlyQualified");
        modelState["IsRecentlyQualified"]!.Errors.Count.Should().Be(1);
        modelState["IsRecentlyQualified"]!.Errors[0].ErrorMessage.Should()
            .Be("Select if the user completed their social work qualification within the last 3 years");

        Sut.BackLinkPath.Should().Be(isFromChangeLink ? "/manage-accounts/eligibility-agency-worker?handler=Change" : "/manage-accounts/eligibility-agency-worker");

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task
        OnPostAsync_WhenCalledWithIsRecentlyQualifiedTrue_RedirectsToEligibilityFundingAvailable()
    {
        // Arrange
        Sut.IsRecentlyQualified = true;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-accounts/eligibility-funding-available");

        MockCreateAccountJourneyService.Verify(x => x.SetIsRecentlyQualified(true), Times.Once);

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithIsRecentlyQualifiedFalse_RedirectsToEligibilityFundingNotAvailable()
    {
        // Arrange
        Sut.IsRecentlyQualified = false;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-accounts/eligibility-funding-not-available");

        MockCreateAccountJourneyService.Verify(x => x.SetIsRecentlyQualified(false), Times.Once);

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public void OnGetChange_WhenCalled_LoadsTheView()
    {
        // Act
        var result = Sut.OnGetChange();

        // Assert
        result.Should().BeOfType<PageResult>();

        Sut.BackLinkPath.Should().Be("/manage-accounts/eligibility-agency-worker?handler=Change");
        Sut.FromChangeLink.Should().BeTrue();
        MockCreateAccountJourneyService.Verify(x => x.GetIsRecentlyQualified(), Times.Once);
        VerifyAllNoOtherCalls();
    }
}
