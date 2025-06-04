using Dfe.Sww.Ecf.Frontend.Pages.ManageUsers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Validation;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageUsers;

public class EligibilityStatutoryWorkDropoutPageTests : ManageUsersPageTestBase<EligibilityStatutoryWorkDropout>
{
    private EligibilityStatutoryWorkDropout Sut { get; }

    public EligibilityStatutoryWorkDropoutPageTests()
    {
        Sut = new EligibilityStatutoryWorkDropout(
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
        Sut.BackLinkPath.Should().Be("/manage-users/eligibility-statutory-work");

        VerifyAllNoOtherCalls();
    }
}
