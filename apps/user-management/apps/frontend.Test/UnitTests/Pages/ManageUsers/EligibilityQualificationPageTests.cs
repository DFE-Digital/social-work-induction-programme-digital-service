using Dfe.Sww.Ecf.Frontend.Pages.ManageUsers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Validation;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageUsers;

public class EligibilityQualificationPageTests : ManageUsersPageTestBase<EligibilityQualification>
{
    private EligibilityQualification Sut { get; }

    public EligibilityQualificationPageTests()
    {
        Sut = new EligibilityQualification(
            MockCreateUserJourneyService.Object,
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

        Sut.BackLinkPath.Should().Be("/manage-users/eligibility-agency-worker");
        MockCreateUserJourneyService.Verify(x => x.GetIsQualifiedWithin3Years(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void OnGet_WhenCalledWithIsQualifiedWithin3Years_LoadsTheViewWithPrepopulatedValue(bool? isQualifiedWithin3Years)
    {
        // Arrange
        MockCreateUserJourneyService.Setup(x => x.GetIsQualifiedWithin3Years())
            .Returns(isQualifiedWithin3Years);

        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();
        Sut.IsQualifiedWithin3Years.Should().Be(isQualifiedWithin3Years);

        MockCreateUserJourneyService.Verify(x => x.GetIsQualifiedWithin3Years(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task
        OnPostAsync_WhenCalledWithNullIsQualifiedWithin3Years_ReturnsErrorsAndRedirectsToEligibilityAgencyWorker()
    {
        // Arrange
        Sut.IsQualifiedWithin3Years = null;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);
        modelStateKeys.Should().Contain("IsQualifiedWithin3Years");
        modelState["IsQualifiedWithin3Years"]!.Errors.Count.Should().Be(1);
        modelState["IsQualifiedWithin3Years"]!.Errors[0].ErrorMessage.Should()
            .Be("Select if the user completed their social work qualification within the last 3 years");

        Sut.BackLinkPath.Should().Be("/manage-users/eligibility-agency-worker");

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task
        OnPostAsync_WhenCalledWithIsQualifiedWithin3YearsTrue_RedirectsToEligibilityFundingAvailable()
    {
        // Arrange
        Sut.IsQualifiedWithin3Years = true;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-users/eligibility-funding-available");

        MockCreateUserJourneyService.Verify(x => x.SetIsQualifiedWithin3Years(true), Times.Once);

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithIsQualifiedWithin3YearsFalse_RedirectsToEligibilityFundingNotAvailable()
    {
        // Arrange
        Sut.IsQualifiedWithin3Years = false;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-users/eligibility-funding-not-available");

        MockCreateUserJourneyService.Verify(x => x.SetIsQualifiedWithin3Years(false), Times.Once);

        VerifyAllNoOtherCalls();
    }
}
