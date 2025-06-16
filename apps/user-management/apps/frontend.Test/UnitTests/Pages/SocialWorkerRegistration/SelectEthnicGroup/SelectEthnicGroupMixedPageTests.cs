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

public class SelectEthnicGroupMixedPageTests : SocialWorkerRegistrationPageTestBase
{
    private SelectEthnicGroupMixed Sut { get; }

    public SelectEthnicGroupMixedPageTests()
    {
        Sut = new(new FakeLinkGenerator(), MockRegisterSocialWorkerJourneyService.Object, MockAuthServiceClient.Object,
            new SelectEthnicGroupMixedValidator());
    }

    [Fact]
    public async Task OnGetAsync_WhenCalled_LoadsTheView()
    {
        // Arrange
        var ethnicGroupMixed = EthnicGroupMixed.WhiteAndBlackCaribbean;
        var otherEthnicGroupMixed = "Test Value";

        MockAuthServiceClient.Setup(x => x.HttpContextService.GetPersonId()).Returns(PersonId);
        MockRegisterSocialWorkerJourneyService.Setup(x => x.EthnicGroups.GetEthnicGroupMixedAsync(PersonId)).ReturnsAsync(ethnicGroupMixed);
        MockRegisterSocialWorkerJourneyService.Setup(x => x.EthnicGroups.GetOtherEthnicGroupMixedAsync(PersonId)).ReturnsAsync(otherEthnicGroupMixed);

        // Act
        var result = await Sut.OnGetAsync();

        // Assert
        Sut.SelectedEthnicGroupMixed.Should().Be(ethnicGroupMixed);
        Sut.OtherEthnicGroupMixed.Should().Be(otherEthnicGroupMixed);
        Sut.BackLinkPath.Should().Be("/social-worker-registration/select-ethnic-group");
        result.Should().BeOfType<PageResult>();

        MockAuthServiceClient.Verify(x => x.HttpContextService.GetPersonId(), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.EthnicGroups.GetEthnicGroupMixedAsync(PersonId), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.EthnicGroups.GetOtherEthnicGroupMixedAsync(PersonId), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithValidValues_SavesValuesAndRedirectsUser()
    {
        // Arrange
        MockAuthServiceClient.Setup(x => x.HttpContextService.GetPersonId()).Returns(PersonId);

        var ethnicGroup = EthnicGroupMixed.WhiteAndBlackCaribbean;
        Sut.SelectedEthnicGroupMixed = ethnicGroup;
        var otherEthnicGroupMixed = "test value";
        Sut.OtherEthnicGroupMixed = otherEthnicGroupMixed;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/social-worker-registration/select-date-of-birth"); // TODO update this ECSW disability page

        MockAuthServiceClient.Verify(x => x.HttpContextService.GetPersonId(), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.EthnicGroups.SetEthnicGroupMixedAsync(PersonId, ethnicGroup), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.EthnicGroups.SetOtherEthnicGroupMixedAsync(PersonId, otherEthnicGroupMixed), Times.Once);
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
