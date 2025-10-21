using Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Validation;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageAccounts;

public class EligibilitySocialWorkEnglandAsyeDropoutPageTests : ManageAccountsPageTestBase<EligibilitySocialWorkEnglandAsyeDropout>
{
    private EligibilitySocialWorkEnglandAsyeDropout Sut { get; }

    public EligibilitySocialWorkEnglandAsyeDropoutPageTests()
    {
        Sut = new EligibilitySocialWorkEnglandAsyeDropout(
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
        Sut.BackLinkPath.Should().Be("/manage-accounts/eligibility-social-work-england");
        Sut.FromChangeLink.Should().BeFalse();
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public void OnGetChange_WhenCalled_LoadsTheView()
    {
        // Act
        var result = Sut.OnGetChange();

        // Assert
        result.Should().BeOfType<PageResult>();

        Sut.BackLinkPath.Should().Be("/manage-accounts/eligibility-social-work-england?handler=Change");
        Sut.FromChangeLink.Should().BeTrue();
        VerifyAllNoOtherCalls();
    }
}
