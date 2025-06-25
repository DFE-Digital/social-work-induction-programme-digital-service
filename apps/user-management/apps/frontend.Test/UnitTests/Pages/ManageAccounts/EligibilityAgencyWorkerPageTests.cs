using Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Validation;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageAccounts;

public class EligibilityAgencyWorkerPageTests : ManageAccountsPageTestBase<EligibilityAgencyWorker>
{
    private EligibilityAgencyWorker Sut { get; }

    public EligibilityAgencyWorkerPageTests()
    {
        Sut = new EligibilityAgencyWorker(
            MockCreateAccountJourneyService.Object,
            new FakeLinkGenerator(),
            new EligibilityAgencyWorkerValidator()
        );
    }

    [Fact]
    public void OnGet_WhenCalled_LoadsTheView()
    {
        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();

        Sut.BackLinkPath.Should().Be("/manage-accounts/eligibility-statutory-work");
        MockCreateAccountJourneyService.Verify(x => x.GetIsAgencyWorker(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void OnGet_WhenCalledWithIsAgencyWorkerPopulated_LoadsTheViewWithPrepopulatedValue(bool? isAgencyWorker)
    {
        // Arrange
        MockCreateAccountJourneyService.Setup(x => x.GetIsAgencyWorker())
            .Returns(isAgencyWorker);

        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();
        Sut.IsAgencyWorker.Should().Be(isAgencyWorker);

        MockCreateAccountJourneyService.Verify(x => x.GetIsAgencyWorker(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithNullIsAgencyWorker_ReturnsErrorsAndRedirectsToEligibilityAgencyWorker()
    {
        // Arrange
        Sut.IsAgencyWorker = null;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);
        modelStateKeys.Should().Contain("IsAgencyWorker");
        modelState["IsAgencyWorker"]!.Errors.Count.Should().Be(1);
        modelState["IsAgencyWorker"]!.Errors[0].ErrorMessage.Should()
            .Be("Select if the user is an agency worker");

        Sut.BackLinkPath.Should().Be("/manage-accounts/eligibility-statutory-work");

        VerifyAllNoOtherCalls();
    }

    [Theory]
    [InlineData(true, "/manage-accounts/eligibility-funding-not-available?handler=Change")]
    [InlineData(false, "/manage-accounts/eligibility-funding-not-available")]
    public async Task OnPostAsync_WhenCalledWithIsAgencyWorkerTrue_RedirectsToEligibilityFundingNotAvailable(
        bool fromChangeLink,
        string redirectPath
    )
    {
        // Arrange
        Sut.IsAgencyWorker = true;
        Sut.FromChangeLink = fromChangeLink;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be(redirectPath);

        MockCreateAccountJourneyService.Verify(x => x.SetIsAgencyWorker(true), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.SetIsRecentlyQualified(null), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.GetIsAgencyWorker(), Times.Once);

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithIsAgencyWorkerTrueAndPreviousValueSelectedTrue_RedirectsToConfirmAccountDetails()
    {
        // Arrange
        Sut.IsAgencyWorker = true;
        Sut.FromChangeLink = true;
        MockCreateAccountJourneyService.Setup(x => x.GetIsAgencyWorker())
            .Returns(true);

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-accounts/confirm-account-details");

        MockCreateAccountJourneyService.Verify(x => x.SetIsAgencyWorker(true), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.SetIsRecentlyQualified(null), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.GetIsAgencyWorker(), Times.Once);

        VerifyAllNoOtherCalls();
    }

    [Theory]
    [InlineData(true, "/manage-accounts/eligibility-qualification?handler=Change")]
    [InlineData(false, "/manage-accounts/eligibility-qualification")]
    public async Task OnPostAsync_WhenCalledWithIsAgencyWorkerFalse_RedirectsToCorrectEligibilityQualificationPage(
        bool fromChangeLink,
        string redirectPath
    )
    {
        // Arrange
        Sut.IsAgencyWorker = false;
        Sut.FromChangeLink = fromChangeLink;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be(redirectPath);

        MockCreateAccountJourneyService.Verify(x => x.SetIsAgencyWorker(false), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.GetIsAgencyWorker(), Times.Once);

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public void OnGetChange_WhenCalled_LoadsTheView()
    {
        // Act
        var result = Sut.OnGetChange();

        // Assert
        result.Should().BeOfType<PageResult>();

        Sut.BackLinkPath.Should().Be("/manage-accounts/confirm-account-details");
        Sut.FromChangeLink.Should().BeTrue();
        MockCreateAccountJourneyService.Verify(x => x.GetIsAgencyWorker(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostChangeAsync_WhenCalled_HasFromChangeLinkTrue()
    {
        // Act
        _ = await Sut.OnPostChangeAsync();

        // Assert
        Sut.FromChangeLink.Should().BeTrue();
    }
}
