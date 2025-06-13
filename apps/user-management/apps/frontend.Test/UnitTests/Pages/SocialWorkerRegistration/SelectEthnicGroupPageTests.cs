using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.SocialWorkerRegistration;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Validation;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.SocialWorkerRegistration;

public class SelectEthnicGroupPageTests : SocialWorkerRegistrationPageTestBase<SelectEthnicGroup>
{
    private SelectEthnicGroup Sut { get; }

    public SelectEthnicGroupPageTests()
    {
        Sut = new(new FakeLinkGenerator(), MockRegisterSocialWorkerJourneyService.Object, MockAuthServiceClient.Object,
            new SelectEthnicGroupValidator());
    }

    [Fact]
    public async Task OnGetAsync_WhenCalled_LoadsTheView()
    {
        // Arrange
        var ethnicGroup = EthnicGroup.White;

        MockAuthServiceClient.Setup(x => x.HttpContextService.GetPersonId()).Returns(PersonId);
        MockRegisterSocialWorkerJourneyService.Setup(x => x.GetEthnicGroupAsync(PersonId)).ReturnsAsync(ethnicGroup);

        // Act
        var result = await Sut.OnGetAsync();

        // Assert
        Sut.SelectedEthnicGroup.Should().Be(ethnicGroup);
        Sut.BackLinkPath.Should().Be("/social-worker-registration/select-sex-and-gender-identity");
        result.Should().BeOfType<PageResult>();

        MockAuthServiceClient.Verify(x => x.HttpContextService.GetPersonId(), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.GetEthnicGroupAsync(PersonId), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithValidValues_SavesValuesAndRedirectsUser()
    {
        // Arrange
        MockAuthServiceClient.Setup(x => x.HttpContextService.GetPersonId()).Returns(PersonId);

        var ethnicGroup = EthnicGroup.White;
        Sut.SelectedEthnicGroup = ethnicGroup;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/social-worker-registration/select-date-of-birth"); // TODO update this ECSW relevant ethnic group sub page

        MockAuthServiceClient.Verify(x => x.HttpContextService.GetPersonId(), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.SetEthnicGroupAsync(PersonId, ethnicGroup), Times.Once);
        VerifyAllNoOtherCalls();
    }


    [Fact]
    public async Task OnPostAsync_WhenCalledWithInvalidValues_ReturnsValidationErrors()
    {
        // Arrange
        Sut.SelectedEthnicGroup = null;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);

        modelStateKeys.Should().Contain("SelectedEthnicGroup");
        modelState["SelectedEthnicGroup"]!.Errors.Count.Should().Be(1);
        modelState["SelectedEthnicGroup"]!.Errors[0].ErrorMessage.Should()
            .Be("Select your ethnic group");

        Sut.BackLinkPath.Should().Be("/social-worker-registration/select-sex-and-gender-identity");

        VerifyAllNoOtherCalls();
    }
}
