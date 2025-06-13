using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Validation;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;
using Index = Dfe.Sww.Ecf.Frontend.Pages.SocialWorkerRegistration.SelectEthnicGroup.Index;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.SocialWorkerRegistration.SelectEthnicGroup;

public class SelectEthnicGroupIndexPageTests : SocialWorkerRegistrationPageTestBase
{
    private Index Sut { get; }

    public SelectEthnicGroupIndexPageTests()
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

    [Theory]
    [InlineData(EthnicGroup.White, "/social-worker-registration/select-ethnic-group/white")]
    [InlineData(EthnicGroup.MixedOrMultipleEthnicGroups, "/social-worker-registration/select-date-of-birth")] // TODO update this when more sub pages are added
    public async Task OnPostAsync_WhenCalledWithValidValues_SavesValuesAndRedirectsUser(EthnicGroup ethnicGroup, string redirectUrl)
    {
        // Arrange
        MockAuthServiceClient.Setup(x => x.HttpContextService.GetPersonId()).Returns(PersonId);

        Sut.SelectedEthnicGroup = ethnicGroup;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be(redirectUrl); // TODO update this when more sub pages are added

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
