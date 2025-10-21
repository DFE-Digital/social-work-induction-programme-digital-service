using Dfe.Sww.Ecf.Frontend.Models;
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
            MockAuthServiceClient.Object,
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
        Sut.FromChangeLink.Should().BeFalse();
        MockCreateAccountJourneyService.Verify(x => x.GetIsRegisteredWithSocialWorkEngland(), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.GetAccountDetails(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void OnGet_WhenCalledWithIsRegisteredWithSocialWorkEnglandPopulated_LoadsTheViewWithPrepopulatedValue(bool? isRegisteredWithSocialWorkEngland)
    {
        // Arrange
        MockCreateAccountJourneyService.Setup(x => x.GetIsRegisteredWithSocialWorkEngland())
            .Returns(isRegisteredWithSocialWorkEngland);

        // Act
        var result = Sut.OnGet();

        // Assert
        result.Should().BeOfType<PageResult>();
        Sut.IsRegisteredWithSocialWorkEngland.Should().Be(isRegisteredWithSocialWorkEngland);

        MockCreateAccountJourneyService.Verify(x => x.GetIsRegisteredWithSocialWorkEngland(), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.GetAccountDetails(), Times.Once);
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
    public async Task OnPostAsync_WhenCalledWithIsRegisteredWithSocialWorkEnglandTrue_RedirectsToEligibilityStatutoryWork()
    {
        // Arrange
        var sweId = "SW123";
        var accountDetails = new AccountDetails { SocialWorkEnglandNumber = sweId };

        Sut.SocialWorkerNumber = sweId;
        Sut.IsRegisteredWithSocialWorkEngland = true;

        MockAuthServiceClient.Setup(x => x.AsyeSocialWorker.ExistsAsync(sweId)).ReturnsAsync(false);

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-accounts/eligibility-statutory-work");

        MockCreateAccountJourneyService.Verify(x => x.SetAccountDetails(MoqHelpers.ShouldBeEquivalentTo(accountDetails)), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.SetIsRegisteredWithSocialWorkEngland(true), Times.Once);
        MockAuthServiceClient.Verify(x => x.AsyeSocialWorker.ExistsAsync(sweId), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.SetIsEnrolledInAsye(false), Times.Once);

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithSweIdThatIsEnrolledOnAsye_RedirectsToAsyeDropoutPage()
    {
        // Arrange
        var sweId = "SW123";
        var accountDetails = new AccountDetails { SocialWorkEnglandNumber = sweId };

        Sut.SocialWorkerNumber = sweId;
        Sut.IsRegisteredWithSocialWorkEngland = true;

        MockAuthServiceClient.Setup(x => x.AsyeSocialWorker.ExistsAsync(sweId)).ReturnsAsync(true);

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/manage-accounts/eligibility-social-work-england-asye-dropout");

        MockCreateAccountJourneyService.Verify(x => x.SetAccountDetails(MoqHelpers.ShouldBeEquivalentTo(accountDetails)), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.SetIsRegisteredWithSocialWorkEngland(true), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.SetIsEnrolledInAsye(true), Times.Once);
        MockAuthServiceClient.Verify(x => x.AsyeSocialWorker.ExistsAsync(sweId), Times.Once);

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
        redirectResult!.Url.Should().Be("/manage-accounts/eligibility-social-work-england-dropout");

        MockCreateAccountJourneyService.Verify(x => x.SetIsRegisteredWithSocialWorkEngland(false), Times.Once);

        VerifyAllNoOtherCalls();
    }

    [Theory]
    [InlineData(true, "/manage-accounts/confirm-account-details")]
    [InlineData(false, "/manage-accounts/eligibility-social-work-england-dropout?handler=Change")]
    public async Task
        OnPostAsync_WhenCalledFromChangeLink_RedirectsToRelevantPage(bool isRegisteredWithSocialWorkEngland, string redirectPath)
    {
        // Arrange
        var sweId = "SW123";
        var accountDetails = new AccountDetails { SocialWorkEnglandNumber = sweId };
        Sut.SocialWorkerNumber = sweId;
        Sut.FromChangeLink = true;
        Sut.IsRegisteredWithSocialWorkEngland = isRegisteredWithSocialWorkEngland;

        MockAuthServiceClient.Setup(x => x.AsyeSocialWorker.ExistsAsync(sweId)).ReturnsAsync(false);

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be(redirectPath);

        MockCreateAccountJourneyService.Verify(x => x.SetIsRegisteredWithSocialWorkEngland(isRegisteredWithSocialWorkEngland), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.SetAccountDetails(
                MoqHelpers.ShouldBeEquivalentTo(accountDetails)),
            isRegisteredWithSocialWorkEngland
                ? Times.Once
                : Times.Never);
        MockAuthServiceClient.Verify(x => x.AsyeSocialWorker.ExistsAsync(sweId),
            isRegisteredWithSocialWorkEngland
                ? Times.Once
                : Times.Never);
        MockCreateAccountJourneyService.Verify(x => x.SetIsEnrolledInAsye(false),
            isRegisteredWithSocialWorkEngland
                ? Times.Once
                : Times.Never);

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public void OnGetChange_WhenCalled_LoadsTheView()
    {
        // Act
        var result = Sut.OnGetChange();

        // Assert
        result.Should().BeOfType<PageResult>();

        Sut.BackLinkPath.Should().Be("/manage-accounts/confirm-account-details");
        Sut.FromChangeLink.Should().BeTrue();
        MockCreateAccountJourneyService.Verify(x => x.GetIsRegisteredWithSocialWorkEngland(), Times.Once);
        MockCreateAccountJourneyService.Verify(x => x.GetAccountDetails(), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostChangeAsync_WhenCalled_HasFromChangeLinkTrue()
    {
        // Act
        _ = await Sut.OnPostChangeAsync();

        // Assert
        Sut.FromChangeLink.Should().BeTrue();
    }
}
