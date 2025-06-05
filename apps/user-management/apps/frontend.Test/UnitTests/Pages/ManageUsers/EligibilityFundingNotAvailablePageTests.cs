using Dfe.Sww.Ecf.Frontend.Pages.ManageUsers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageUsers;

public class EligibilityFundingNotAvailablePageTests : ManageUsersPageTestBase<EligibilityFundingNotAvailable>
{
    private EligibilityFundingNotAvailable Sut { get; }

    public EligibilityFundingNotAvailablePageTests()
    {
        Sut = new EligibilityFundingNotAvailable(
            MockCreateUserJourneyService.Object,
            new FakeLinkGenerator()
        );
    }

    [Theory]
    [InlineData(true, "/manage-users/eligibility-agency-worker")]
    [InlineData(false, "/manage-users/eligibility-qualification")]
    public void OnGet_WhenCalled_LoadsTheView(bool isAgencyWorker, string expectedBackLink)
    {
        MockCreateUserJourneyService.Setup(x => x.GetIsAgencyWorker()).Returns(isAgencyWorker);

        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();
        Sut.BackLinkPath.Should().Be(expectedBackLink);

        MockCreateUserJourneyService.Verify(x => x.GetIsAgencyWorker(), Times.Once);
        VerifyAllNoOtherCalls();
    }
}
