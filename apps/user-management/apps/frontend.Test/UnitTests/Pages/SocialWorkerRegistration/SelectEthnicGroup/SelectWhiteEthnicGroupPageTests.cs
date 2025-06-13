using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.SocialWorkerRegistration.SelectEthnicGroup;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Validation;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;
using Index = Dfe.Sww.Ecf.Frontend.Pages.SocialWorkerRegistration.SelectEthnicGroup.Index;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.SocialWorkerRegistration.SelectEthnicGroup;

public class SelectWhiteEthnicGroupPageTests : SocialWorkerRegistrationPageTestBase<Index>
{
    private SelectWhiteEthnicGroup Sut { get; }

    public SelectWhiteEthnicGroupPageTests()
    {
        Sut = new(new FakeLinkGenerator(), MockRegisterSocialWorkerJourneyService.Object, MockAuthServiceClient.Object,
            new SelectWhiteEthnicGroupValidator());
    }

    [Fact]
    public async Task OnGetAsync_WhenCalled_LoadsTheView()
    {
        // Arrange
        var whiteEthnicGroup = EthnicGroupWhite.EnglishWelshScottishNorthernIrishOrBritish;
        var otherWhiteEthnicGroup = "Test Value";

        MockAuthServiceClient.Setup(x => x.HttpContextService.GetPersonId()).Returns(PersonId);
        MockRegisterSocialWorkerJourneyService.Setup(x => x.GetWhiteEthnicGroupAsync(PersonId)).ReturnsAsync(whiteEthnicGroup);
        MockRegisterSocialWorkerJourneyService.Setup(x => x.GetOtherWhiteEthnicGroupAsync(PersonId)).ReturnsAsync(otherWhiteEthnicGroup);

        // Act
        var result = await Sut.OnGetAsync();

        // Assert
        Sut.SelectedEthnicGroupWhite.Should().Be(whiteEthnicGroup);
        Sut.OtherWhiteEthnicGroup.Should().Be(otherWhiteEthnicGroup);
        Sut.BackLinkPath.Should().Be("/social-worker-registration/select-ethnic-group");
        result.Should().BeOfType<PageResult>();

        MockAuthServiceClient.Verify(x => x.HttpContextService.GetPersonId(), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.GetWhiteEthnicGroupAsync(PersonId), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.GetOtherWhiteEthnicGroupAsync(PersonId), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithValidValues_SavesValuesAndRedirectsUser()
    {
        // Arrange
        MockAuthServiceClient.Setup(x => x.HttpContextService.GetPersonId()).Returns(PersonId);

        var ethnicGroup = EthnicGroupWhite.EnglishWelshScottishNorthernIrishOrBritish;
        Sut.SelectedEthnicGroupWhite = ethnicGroup;
        var otherWhiteEthnicGroup = "test value";
        Sut.OtherWhiteEthnicGroup = otherWhiteEthnicGroup;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/social-worker-registration/select-date-of-birth"); // TODO update this ECSW disability page

        MockAuthServiceClient.Verify(x => x.HttpContextService.GetPersonId(), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.SetEthnicGroupWhiteAsync(PersonId, ethnicGroup), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.SetOtherWhiteEthnicGroupAsync(PersonId, otherWhiteEthnicGroup), Times.Once);
        VerifyAllNoOtherCalls();
    }


    [Fact]
    public async Task OnPostAsync_WhenCalledWithInvalidValues_ReturnsValidationErrors()
    {
        // Arrange
        Sut.SelectedEthnicGroupWhite = null;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);

        modelStateKeys.Should().Contain("SelectedEthnicGroupWhite");
        modelState["SelectedEthnicGroupWhite"]!.Errors.Count.Should().Be(1);
        modelState["SelectedEthnicGroupWhite"]!.Errors[0].ErrorMessage.Should()
            .Be("Select an option that best describes your White background");

        Sut.BackLinkPath.Should().Be("/social-worker-registration/select-ethnic-group");

        VerifyAllNoOtherCalls();
    }
}
