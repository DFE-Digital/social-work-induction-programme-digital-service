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

public class SelectMixedEthnicGroupPageTests : SocialWorkerRegistrationPageTestBase
{
    private SelectMixedEthnicGroup Sut { get; }

    public SelectMixedEthnicGroupPageTests()
    {
        Sut = new(new FakeLinkGenerator(), MockRegisterSocialWorkerJourneyService.Object, MockAuthServiceClient.Object,
            new SelectMixedEthnicGroupValidator());
    }

    [Fact]
    public async Task OnGetAsync_WhenCalled_LoadsTheView()
    {
        // Arrange
        var mixedEthnicGroup = EthnicGroupMixed.WhiteAndBlackCaribbean;
        var otherMixedEthnicGroup = "Test Value";

        MockAuthServiceClient.Setup(x => x.HttpContextService.GetPersonId()).Returns(PersonId);
        MockRegisterSocialWorkerJourneyService.Setup(x => x.GetMixedEthnicGroupAsync(PersonId)).ReturnsAsync(mixedEthnicGroup);
        MockRegisterSocialWorkerJourneyService.Setup(x => x.GetOtherMixedEthnicGroupAsync(PersonId)).ReturnsAsync(otherMixedEthnicGroup);

        // Act
        var result = await Sut.OnGetAsync();

        // Assert
        Sut.SelectedEthnicGroupMixed.Should().Be(mixedEthnicGroup);
        Sut.OtherMixedEthnicGroup.Should().Be(otherMixedEthnicGroup);
        Sut.BackLinkPath.Should().Be("/social-worker-registration/select-ethnic-group");
        result.Should().BeOfType<PageResult>();

        MockAuthServiceClient.Verify(x => x.HttpContextService.GetPersonId(), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.GetMixedEthnicGroupAsync(PersonId), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.GetOtherMixedEthnicGroupAsync(PersonId), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithValidValues_SavesValuesAndRedirectsUser()
    {
        // Arrange
        MockAuthServiceClient.Setup(x => x.HttpContextService.GetPersonId()).Returns(PersonId);

        var ethnicGroup = EthnicGroupMixed.WhiteAndBlackCaribbean;
        Sut.SelectedEthnicGroupMixed = ethnicGroup;
        var otherMixedEthnicGroup = "test value";
        Sut.OtherMixedEthnicGroup = otherMixedEthnicGroup;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/social-worker-registration/select-date-of-birth"); // TODO update this ECSW disability page

        MockAuthServiceClient.Verify(x => x.HttpContextService.GetPersonId(), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.SetEthnicGroupMixedAsync(PersonId, ethnicGroup), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.SetOtherMixedEthnicGroupAsync(PersonId, otherMixedEthnicGroup), Times.Once);
        VerifyAllNoOtherCalls();
    }


    [Fact]
    public async Task OnPostAsync_WhenCalledWithInvalidValues_ReturnsValidationErrors()
    {
        // Arrange
        Sut.SelectedEthnicGroupMixed = null;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);

        modelStateKeys.Should().Contain("SelectedEthnicGroupMixed");
        modelState["SelectedEthnicGroupMixed"]!.Errors.Count.Should().Be(1);
        modelState["SelectedEthnicGroupMixed"]!.Errors[0].ErrorMessage.Should()
            .Be("Select an option that best describes your mixed or multiple ethnic groups background");

        Sut.BackLinkPath.Should().Be("/social-worker-registration/select-ethnic-group");

        VerifyAllNoOtherCalls();
    }
}
