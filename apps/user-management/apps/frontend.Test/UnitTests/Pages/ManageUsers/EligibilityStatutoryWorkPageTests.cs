using Dfe.Sww.Ecf.Frontend.Pages.ManageUsers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Validation;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageUsers;

public class EligibilityStatutoryWorkPageTests : ManageUsersPageTestBase<EligibilityStatutoryWork>
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
        Sut.IsStatutoryWorker.Should().BeNull();
        Sut.BackLinkPath.Should().Be("/manage-users/eligibility-social-work-england");

        MockCreateAccountJourneyService.Verify(x => x.GetIsStatutoryWorker(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void OnGet_WhenCalledWithIsStatutoryWorkerSet_LoadsTheViewWithPreselectedOption(bool isStatutoryWorker)
    {
        // Arrange
        MockCreateAccountJourneyService.Setup(x => x.GetIsStatutoryWorker())
            .Returns(isStatutoryWorker);

        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();
        Sut.IsStatutoryWorker.Should().Be(isStatutoryWorker);

        MockCreateAccountJourneyService.Verify(x => x.GetIsStatutoryWorker(), Times.Once);
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

        Sut.BackLinkPath.Should().Be("/manage-users/eligibility-social-work-england");

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
        redirectResult!.Url.Should().Be("/manage-users/eligibility-agency-worker");

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
        redirectResult!.Url.Should().Be("/manage-users/eligibility-statutory-work-dropout");

        MockCreateAccountJourneyService.Verify(x => x.SetIsStatutoryWorker(false), Times.Once);

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task
        OnPostAsync_WhenCalledWithIsRegisteredWithSocialWorkEnglandFalse_RedirectsToEligibilityDropoutPage()
    {
        // Arrange
        Sut.IsStatutoryWorker = false;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-users/eligibility-statutory-work-dropout");

        MockCreateAccountJourneyService.Verify(x => x.SetIsStatutoryWorker(false), Times.Once);

        VerifyAllNoOtherCalls();
    }
}
