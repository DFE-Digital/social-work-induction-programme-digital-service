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
            new FakeLinkGenerator()
        );
    }

    [Fact]
    public void OnGet_WhenCalled_LoadsTheView()
    {
        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();
        Sut.BackLinkPath.Should().Be("/manage-accounts/eligibility-qualification");

        VerifyAllNoOtherCalls();
    }
}
