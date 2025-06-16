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

public class SelectEthnicGroupAsianPageTests : SocialWorkerRegistrationPageTestBase
{
    private SelectEthnicGroupAsian Sut { get; }

    public SelectEthnicGroupAsianPageTests()
    {
        Sut = new(new FakeLinkGenerator(), MockRegisterSocialWorkerJourneyService.Object, MockAuthServiceClient.Object,
            new SelectEthnicGroupAsianValidator());
    }

    [Fact]
    public async Task OnGetAsync_WhenCalled_LoadsTheView()
    {
        // Arrange
        var ethnicGroupAsian = EthnicGroupAsian.Indian;
        var otherEthnicGroupAsian = "Test Value";

        MockAuthServiceClient.Setup(x => x.HttpContextService.GetPersonId()).Returns(PersonId);
        MockRegisterSocialWorkerJourneyService.Setup(x => x.GetEthnicGroupAsianAsync(PersonId)).ReturnsAsync(ethnicGroupAsian);
        MockRegisterSocialWorkerJourneyService.Setup(x => x.GetOtherEthnicGroupAsianAsync(PersonId)).ReturnsAsync(otherEthnicGroupAsian);

        // Act
        var result = await Sut.OnGetAsync();

        // Assert
        Sut.SelectedEthnicGroupAsian.Should().Be(ethnicGroupAsian);
        Sut.OtherEthnicGroupAsian.Should().Be(otherEthnicGroupAsian);
        Sut.BackLinkPath.Should().Be("/social-worker-registration/select-ethnic-group");
        result.Should().BeOfType<PageResult>();

        MockAuthServiceClient.Verify(x => x.HttpContextService.GetPersonId(), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.GetEthnicGroupAsianAsync(PersonId), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.GetOtherEthnicGroupAsianAsync(PersonId), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithValidValues_SavesValuesAndRedirectsUser()
    {
        // Arrange
        MockAuthServiceClient.Setup(x => x.HttpContextService.GetPersonId()).Returns(PersonId);

        var ethnicGroup = EthnicGroupAsian.Indian;
        Sut.SelectedEthnicGroupAsian = ethnicGroup;
        var otherEthnicGroupAsian = "test value";
        Sut.OtherEthnicGroupAsian = otherEthnicGroupAsian;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/social-worker-registration/select-date-of-birth"); // TODO update this ECSW disability page

        MockAuthServiceClient.Verify(x => x.HttpContextService.GetPersonId(), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.SetEthnicGroupAsianAsync(PersonId, ethnicGroup), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.SetOtherEthnicGroupAsianAsync(PersonId, otherEthnicGroupAsian), Times.Once);
        VerifyAllNoOtherCalls();
    }


    [Fact]
    public async Task OnPostAsync_WhenCalledWithInvalidValues_ReturnsValidationErrors()
    {
        // Arrange
        Sut.SelectedEthnicGroupAsian = null;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);

        modelStateKeys.Should().Contain("SelectedEthnicGroupAsian");
        modelState["SelectedEthnicGroupAsian"]!.Errors.Count.Should().Be(1);
        modelState["SelectedEthnicGroupAsian"]!.Errors[0].ErrorMessage.Should()
            .Be("Select an option that best describes your Asian or Asian British background");

        Sut.BackLinkPath.Should().Be("/social-worker-registration/select-ethnic-group");

        VerifyAllNoOtherCalls();
    }
}
