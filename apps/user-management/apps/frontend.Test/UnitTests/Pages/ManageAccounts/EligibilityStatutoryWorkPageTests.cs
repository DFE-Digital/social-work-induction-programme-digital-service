using Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Validation;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageAccounts;

public class EligibilityStatutoryWorkPageTests : ManageAccountsPageTestBase<EligibilityStatutoryWork>
{
    private EligibilityStatutoryWork Sut { get; }

    public EligibilityStatutoryWorkPageTests()
    {
        Sut = new EligibilityStatutoryWork(
            MockCreateAccountJourneyService.Object,
            new FakeLinkGenerator(),
            new EligibilityStatutoryWorkValidator()
        );
    }

    [Fact]
    public void OnGet_WhenCalled_LoadsTheView()
    {
        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();

        Sut.BackLinkPath.Should().Be("/manage-accounts/eligibility-social-work-england");

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task
        OnPostAsync_WhenCalledWithNullIsStatutoryWorker_ReturnsErrorsAndRedirectsToEligibilitySocialWorkEngland()
    {
        // Arrange
        Sut.IsStatutoryWorker = null;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);
        modelStateKeys.Should().Contain("IsStatutoryWorker");
        modelState["IsStatutoryWorker"]!.Errors.Count.Should().Be(1);
        modelState["IsStatutoryWorker"]!.Errors[0].ErrorMessage.Should()
            .Be("Select if the user currently works in statutory child and family social work in England");

        Sut.BackLinkPath.Should().Be("/manage-accounts/eligibility-social-work-england");

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task
        OnPostAsync_WhenCalledWithIsStatutoryWorkerTrue_RedirectsToEligibilityAgencyWork()
    {
        // Arrange
        Sut.IsStatutoryWorker = true;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        // TODO: redirect to eligibility agency work page in SWIP-580
        // redirectResult!.Url.Should().Be("/manage-accounts/eligibility-agency-work");
        redirectResult!.Url.Should().Be("/manage-accounts/add-account-details");

        MockCreateAccountJourneyService.Verify(x => x.SetIsStatutoryWorker(true), Times.Once);

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task
        OnPostAsync_WhenCalledWithIsStatutoryWorkerFalse_RedirectsToEligibilityStatutoryWorkDropoutPage()
    {
        // Arrange
        Sut.IsStatutoryWorker = false;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        // TODO: redirect to eligibility statutory work dropout page in SWIP-592
        // redirectResult!.Url.Should().Be("/manage-accounts/eligibility-statutory-work-dropout");
        redirectResult!.Url.Should().Be("/manage-accounts/eligibility-social-work-england-dropout");

        MockCreateAccountJourneyService.Verify(x => x.SetIsStatutoryWorker(false), Times.Once);

        VerifyAllNoOtherCalls();
    }
}
