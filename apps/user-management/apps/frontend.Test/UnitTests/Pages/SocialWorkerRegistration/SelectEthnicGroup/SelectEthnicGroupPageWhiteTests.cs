using Dfe.Sww.Ecf.Frontend.Models.RegisterSocialWorker;
using Dfe.Sww.Ecf.Frontend.Pages.SocialWorkerRegistration.SelectEthnicGroup;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Validation.RegisterSocialWorker;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.SocialWorkerRegistration.SelectEthnicGroup;

public class SelectEthnicGroupPageWhiteTests : SocialWorkerRegistrationPageTestBase
{
    private SelectEthnicGroupWhite Sut { get; }

    public SelectEthnicGroupPageWhiteTests()
    {
        Sut = new(new FakeLinkGenerator(), MockRegisterSocialWorkerJourneyService.Object, MockAuthServiceClient.Object,
            new SelectEthnicGroupWhiteValidator());
    }

    [Fact]
    public async Task OnGetAsync_WhenCalled_LoadsTheView()
    {
        // Arrange
        var ethnicGroupWhite = EthnicGroupWhite.EnglishWelshScottishNorthernIrishOrBritish;
        var otherEthnicGroupWhite = "Test Value";

        MockAuthServiceClient.Setup(x => x.HttpContextService.GetPersonId()).Returns(PersonId);
        MockRegisterSocialWorkerJourneyService.Setup(x => x.EthnicGroups.GetEthnicGroupWhiteAsync(PersonId)).ReturnsAsync(ethnicGroupWhite);
        MockRegisterSocialWorkerJourneyService.Setup(x => x.EthnicGroups.GetOtherEthnicGroupWhiteAsync(PersonId)).ReturnsAsync(otherEthnicGroupWhite);

        // Act
        var result = await Sut.OnGetAsync();

        // Assert
        Sut.SelectedEthnicGroupWhite.Should().Be(ethnicGroupWhite);
        Sut.OtherEthnicGroupWhite.Should().Be(otherEthnicGroupWhite);
        Sut.BackLinkPath.Should().Be("/social-worker-registration/select-ethnic-group");
        result.Should().BeOfType<PageResult>();

        MockAuthServiceClient.Verify(x => x.HttpContextService.GetPersonId(), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.EthnicGroups.GetEthnicGroupWhiteAsync(PersonId), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.EthnicGroups.GetOtherEthnicGroupWhiteAsync(PersonId), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithValidValues_SavesValuesAndRedirectsUser()
    {
        // Arrange
        MockAuthServiceClient.Setup(x => x.HttpContextService.GetPersonId()).Returns(PersonId);

        var ethnicGroupWhite = EthnicGroupWhite.EnglishWelshScottishNorthernIrishOrBritish;
        Sut.SelectedEthnicGroupWhite = ethnicGroupWhite;
        var otherEthnicGroupWhite = "test value";
        Sut.OtherEthnicGroupWhite = otherEthnicGroupWhite;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/social-worker-registration/select-disability");

        MockAuthServiceClient.Verify(x => x.HttpContextService.GetPersonId(), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.EthnicGroups.SetEthnicGroupWhiteAsync(PersonId, ethnicGroupWhite), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.EthnicGroups.SetOtherEthnicGroupWhiteAsync(PersonId, otherEthnicGroupWhite), Times.Once);
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

    [Fact]
    public async Task OnGetChange_WhenCalled_LoadsTheView()
    {
        // Arrange
        MockAuthServiceClient.Setup(x => x.HttpContextService.GetPersonId()).Returns(PersonId);

        // Act
        var result = await Sut.OnGetChangeAsync();

        // Assert
        result.Should().BeOfType<PageResult>();

        Sut.BackLinkPath.Should().Be("/social-worker-registration/select-ethnic-group");
        Sut.FromChangeLink.Should().BeTrue();
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
