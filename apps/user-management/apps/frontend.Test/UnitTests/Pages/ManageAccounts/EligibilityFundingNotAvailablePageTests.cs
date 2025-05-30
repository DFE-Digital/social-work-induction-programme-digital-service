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
    public void OnGet_WhenCalled_LoadsTheView(bool isAgencyWorker, string expectedBackLink)
    {
        MockCreateAccountJourneyService.Setup(x => x.GetIsAgencyWorker()).Returns(isAgencyWorker);

        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();
        Sut.BackLinkPath.Should().Be(expectedBackLink);

        MockCreateAccountJourneyService.Verify(x => x.GetIsAgencyWorker(), Times.Once);
        VerifyAllNoOtherCalls();
    }
}
