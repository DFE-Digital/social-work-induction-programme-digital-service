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
    public async Task
        OnPostAsync_WhenCalledWithNullIsAgencyWorker_ReturnsErrorsAndRedirectsToEligibilityAgencyWorker()
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

    [Fact]
    public async Task
        OnPostAsync_WhenCalledWithIsAgencyWorkerTrue_RedirectsToEligibilityFundingNotAvailable()
    {
        // Arrange
        Sut.IsAgencyWorker = true;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-accounts/eligibility-funding-not-available");

        MockCreateAccountJourneyService.Verify(x => x.SetIsAgencyWorker(true), Times.Once);

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithIsAgencyWorkerFalse_RedirectsToEligibilityQualification()
    {
        // Arrange
        Sut.IsAgencyWorker = false;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-accounts/eligibility-qualification");

        MockCreateAccountJourneyService.Verify(x => x.SetIsAgencyWorker(false), Times.Once);

        VerifyAllNoOtherCalls();
    }
}
