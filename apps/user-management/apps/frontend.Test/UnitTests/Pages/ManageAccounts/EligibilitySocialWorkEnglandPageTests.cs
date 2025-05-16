using Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Validation;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.ManageAccounts;

public class EligibilitySocialWorkEnglandPageTests : ManageAccountsPageTestBase<EligibilitySocialWorkEngland>
{
    private EligibilitySocialWorkEngland Sut { get; }

    public EligibilitySocialWorkEnglandPageTests()
    {
        Sut = new EligibilitySocialWorkEngland(
            MockCreateAccountJourneyService.Object,
            new FakeLinkGenerator(),
            new EligibilitySocialWorkEnglandValidator()
        );
    }

    [Fact]
    public void OnGet_WhenCalled_LoadsTheView()
    {
        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();

        Sut.BackLinkPath.Should().Be("/manage-accounts/eligibility-information");

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task
        OnPostAsync_WhenCalledWithNullIsRegisteredWithSocialWorkEngland_ReturnsErrorsAndRedirectsToEligibilitySocialWorkEngland()
    {
        // Arrange
        Sut.IsRegisteredWithSocialWorkEngland = null;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);
        modelStateKeys.Should().Contain("IsRegisteredWithSocialWorkEngland");
        modelState["IsRegisteredWithSocialWorkEngland"]!.Errors.Count.Should().Be(1);
        modelState["IsRegisteredWithSocialWorkEngland"]!.Errors[0].ErrorMessage.Should()
            .Be("Select if the user is registered with Social Work England");

        Sut.BackLinkPath.Should().Be("/manage-accounts/eligibility-information");

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task
        OnPostAsync_WhenCalledWithIsRegisteredWithSocialWorkEnglandTrue_RedirectsToEligibilityStatutoryWork()
    {
        // Arrange
        Sut.IsRegisteredWithSocialWorkEngland = true;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        // TODO: Update this redirect assertion in SWIP-579
        // redirectResult!.Url.Should().Be("/manage-accounts/eligibility-statutory-work");
        redirectResult!.Url.Should().Be("/manage-accounts/add-account-details");

        MockCreateAccountJourneyService.Verify(x => x.SetIsRegisteredWithSocialWorkEngland(true), Times.Once);

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task
        OnPostAsync_WhenCalledWithIsRegisteredWithSocialWorkEnglandFalse_RedirectsToEligibilityDropoutPage()
    {
        // Arrange
        Sut.IsRegisteredWithSocialWorkEngland = false;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        // TODO: Update this redirect assertion in SWIP-590
        // redirectResult!.Url.Should().Be("/manage-accounts/eligibility-dropout");
        redirectResult!.Url.Should().Be("/manage-accounts/add-account-details");

        MockCreateAccountJourneyService.Verify(x => x.SetIsRegisteredWithSocialWorkEngland(false), Times.Once);

        VerifyAllNoOtherCalls();
    }
}
