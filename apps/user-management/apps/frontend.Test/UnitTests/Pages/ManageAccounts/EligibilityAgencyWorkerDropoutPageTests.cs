using Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Validation;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageAccounts;

public class EligibilityAgencyWorkerDropoutPageTests : ManageAccountsPageTestBase<EligibilityAgencyWorkerDropout>
{
    private EligibilityAgencyWorkerDropout Sut { get; }

    public EligibilityAgencyWorkerDropoutPageTests()
    {
        Sut = new EligibilityAgencyWorkerDropout(
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
        Sut.BackLinkPath.Should().Be("/manage-accounts/eligibility-agency-worker");

        VerifyAllNoOtherCalls();
    }
}
