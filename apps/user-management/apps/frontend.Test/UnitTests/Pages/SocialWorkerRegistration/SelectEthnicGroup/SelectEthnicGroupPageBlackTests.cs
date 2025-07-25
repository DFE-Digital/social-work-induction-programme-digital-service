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

public class SelectEthnicGroupPageBlackTests : SocialWorkerRegistrationPageTestBase
{
    private SelectEthnicGroupBlack Sut { get; }

    public SelectEthnicGroupPageBlackTests()
    {
        Sut = new(new FakeLinkGenerator(), MockRegisterSocialWorkerJourneyService.Object, MockAuthServiceClient.Object,
            new SelectEthnicGroupBlackValidator());
    }

    [Fact]
    public async Task OnGetAsync_WhenCalled_LoadsTheView()
    {
        // Arrange
        var ethnicGroupBlack = EthnicGroupBlack.African;
        var otherEthnicGroupBlack = "Test Value";

        MockAuthServiceClient.Setup(x => x.HttpContextService.GetPersonId()).Returns(PersonId);
        MockRegisterSocialWorkerJourneyService.Setup(x => x.EthnicGroups.GetEthnicGroupBlackAsync(PersonId)).ReturnsAsync(ethnicGroupBlack);
        MockRegisterSocialWorkerJourneyService.Setup(x => x.EthnicGroups.GetOtherEthnicGroupBlackAsync(PersonId)).ReturnsAsync(otherEthnicGroupBlack);

        // Act
        var result = await Sut.OnGetAsync();

        // Assert
        Sut.SelectedEthnicGroupBlack.Should().Be(ethnicGroupBlack);
        Sut.OtherEthnicGroupBlack.Should().Be(otherEthnicGroupBlack);
        Sut.BackLinkPath.Should().Be("/social-worker-registration/select-ethnic-group");
        result.Should().BeOfType<PageResult>();

        MockAuthServiceClient.Verify(x => x.HttpContextService.GetPersonId(), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.EthnicGroups.GetEthnicGroupBlackAsync(PersonId), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.EthnicGroups.GetOtherEthnicGroupBlackAsync(PersonId), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithValidValues_SavesValuesAndRedirectsUser()
    {
        // Arrange
        MockAuthServiceClient.Setup(x => x.HttpContextService.GetPersonId()).Returns(PersonId);

        var ethnicGroupBlack = EthnicGroupBlack.African;
        Sut.SelectedEthnicGroupBlack = ethnicGroupBlack;
        var otherEthnicGroupBlack = "test value";
        Sut.OtherEthnicGroupBlack = otherEthnicGroupBlack;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/social-worker-registration/select-disability");

        MockAuthServiceClient.Verify(x => x.HttpContextService.GetPersonId(), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.EthnicGroups.SetEthnicGroupBlackAsync(PersonId, ethnicGroupBlack), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.EthnicGroups.SetOtherEthnicGroupBlackAsync(PersonId, otherEthnicGroupBlack), Times.Once);
        VerifyAllNoOtherCalls();
    }


    [Fact]
    public async Task OnPostAsync_WhenCalledWithInvalidValues_ReturnsValidationErrors()
    {
        // Arrange
        Sut.SelectedEthnicGroupBlack = null;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);

        modelStateKeys.Should().Contain("SelectedEthnicGroupBlack");
        modelState["SelectedEthnicGroupBlack"]!.Errors.Count.Should().Be(1);
        modelState["SelectedEthnicGroupBlack"]!.Errors[0].ErrorMessage.Should()
            .Be("Select an option that best describes your Black, African, Caribbean or Black British background");

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
