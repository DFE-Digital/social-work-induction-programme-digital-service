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

public class SelectEthnicGroupPageOtherTests : SocialWorkerRegistrationPageTestBase
{
    private SelectEthnicGroupOther Sut { get; }

    public SelectEthnicGroupPageOtherTests()
    {
        Sut = new(new FakeLinkGenerator(), MockRegisterSocialWorkerJourneyService.Object, MockAuthServiceClient.Object,
            new SelectEthnicGroupOtherValidator());
    }

    [Fact]
    public async Task OnGetAsync_WhenCalled_LoadsTheView()
    {
        // Arrange
        var ethnicGroupOther = EthnicGroupOther.Arab;
        var otherEthnicGroupOther = "Test Value";

        MockAuthServiceClient.Setup(x => x.HttpContextService.GetPersonId()).Returns(PersonId);
        MockRegisterSocialWorkerJourneyService.Setup(x => x.EthnicGroups.GetEthnicGroupOtherAsync(PersonId)).ReturnsAsync(ethnicGroupOther);
        MockRegisterSocialWorkerJourneyService.Setup(x => x.EthnicGroups.GetOtherEthnicGroupOtherAsync(PersonId)).ReturnsAsync(otherEthnicGroupOther);

        // Act
        var result = await Sut.OnGetAsync();

        // Assert
        Sut.SelectedEthnicGroupOther.Should().Be(ethnicGroupOther);
        Sut.OtherEthnicGroupOther.Should().Be(otherEthnicGroupOther);
        Sut.BackLinkPath.Should().Be("/social-worker-registration/select-ethnic-group");
        result.Should().BeOfType<PageResult>();

        MockAuthServiceClient.Verify(x => x.HttpContextService.GetPersonId(), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.EthnicGroups.GetEthnicGroupOtherAsync(PersonId), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.EthnicGroups.GetOtherEthnicGroupOtherAsync(PersonId), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithValidValues_SavesValuesAndRedirectsUser()
    {
        // Arrange
        MockAuthServiceClient.Setup(x => x.HttpContextService.GetPersonId()).Returns(PersonId);

        var ethnicGroupOther = EthnicGroupOther.Arab;
        Sut.SelectedEthnicGroupOther = ethnicGroupOther;
        var otherEthnicGroupOther = "test value";
        Sut.OtherEthnicGroupOther = otherEthnicGroupOther;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/social-worker-registration/select-disability");

        MockAuthServiceClient.Verify(x => x.HttpContextService.GetPersonId(), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.EthnicGroups.SetEthnicGroupOtherAsync(PersonId, ethnicGroupOther), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.EthnicGroups.SetOtherEthnicGroupOtherAsync(PersonId, otherEthnicGroupOther), Times.Once);
        VerifyAllNoOtherCalls();
    }


    [Fact]
    public async Task OnPostAsync_WhenCalledWithInvalidValues_ReturnsValidationErrors()
    {
        // Arrange
        Sut.SelectedEthnicGroupOther = null;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);

        modelStateKeys.Should().Contain("SelectedEthnicGroupOther");
        modelState["SelectedEthnicGroupOther"]!.Errors.Count.Should().Be(1);
        modelState["SelectedEthnicGroupOther"]!.Errors[0].ErrorMessage.Should()
            .Be("Select an option that best describes your background");

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
