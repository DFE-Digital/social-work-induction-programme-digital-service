using Dfe.Sww.Ecf.Frontend.Models.RegisterSocialWorker;
using Dfe.Sww.Ecf.Frontend.Pages.SocialWorkerRegistration;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Validation.RegisterSocialWorker;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.SocialWorkerRegistration;

public class SelectRouteIntoSocialWorkPageTests : SocialWorkerRegistrationPageTestBase
{
    private SelectRouteIntoSocialWork Sut { get; }

    public SelectRouteIntoSocialWorkPageTests()
    {
        Sut = new(new FakeLinkGenerator(), MockRegisterSocialWorkerJourneyService.Object, MockAuthServiceClient.Object,
            new SelectRouteIntoSocialWorkValidator());
    }

    [Fact]
    public async Task OnGetAsync_WhenCalled_LoadsTheView()
    {
        // Arrange
        var entryRoute = RouteIntoSocialWork.DegreeApprenticeship;
        var otherRoute = "test value";

        MockAuthServiceClient.Setup(x => x.HttpContextService.GetPersonId()).Returns(PersonId);
        MockRegisterSocialWorkerJourneyService.Setup(x => x.GetRouteIntoSocialWorkAsync(PersonId)).ReturnsAsync(entryRoute);
        MockRegisterSocialWorkerJourneyService.Setup(x => x.GetOtherRouteIntoSocialWorkAsync(PersonId)).ReturnsAsync(otherRoute);

        // Act
        var result = await Sut.OnGetAsync();

        // Assert
        Sut.SelectedRouteIntoSocialWork.Should().Be(entryRoute);
        Sut.OtherRouteIntoSocialWork.Should().Be(otherRoute);
        Sut.BackLinkPath.Should().Be("/social-worker-registration/select-social-work-qualification-end-year");
        result.Should().BeOfType<PageResult>();

        MockAuthServiceClient.Verify(x => x.HttpContextService.GetPersonId(), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.GetRouteIntoSocialWorkAsync(PersonId), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.GetOtherRouteIntoSocialWorkAsync(PersonId), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithValidValues_SavesValuesAndRedirectsUser()
    {
        // Arrange
        MockAuthServiceClient.Setup(x => x.HttpContextService.GetPersonId()).Returns(PersonId);

        var entryRoute = RouteIntoSocialWork.DegreeApprenticeship;
        var otherRoute = "test value";

        Sut.SelectedRouteIntoSocialWork = entryRoute;
        Sut.OtherRouteIntoSocialWork = otherRoute;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/social-worker-registration/check-your-answers");

        MockAuthServiceClient.Verify(x => x.HttpContextService.GetPersonId(), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.SetRouteIntoSocialWorkAsync(PersonId, entryRoute), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.SetOtherRouteIntoSocialWorkAsync(PersonId, otherRoute), Times.Once);
        VerifyAllNoOtherCalls();
    }


    [Fact]
    public async Task OnPostAsync_WhenCalledWithInvalidValues_ReturnsValidationErrors()
    {
        // Arrange
        Sut.SelectedRouteIntoSocialWork = null;
        Sut.OtherRouteIntoSocialWork = null;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);

        modelStateKeys.Should().Contain("SelectedRouteIntoSocialWork");
        modelState["SelectedRouteIntoSocialWork"]!.Errors.Count.Should().Be(1);
        modelState["SelectedRouteIntoSocialWork"]!.Errors[0].ErrorMessage.Should()
            .Be("Select your entry route into social work");

        Sut.BackLinkPath.Should().Be("/social-worker-registration/select-social-work-qualification-end-year");

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithInvalidOtherValues_ReturnsValidationErrors()
    {
        // Arrange
        Sut.SelectedRouteIntoSocialWork = RouteIntoSocialWork.Other;
        Sut.OtherRouteIntoSocialWork = null;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);

        modelStateKeys.Should().Contain("OtherRouteIntoSocialWork");
        modelState["OtherRouteIntoSocialWork"]!.Errors.Count.Should().Be(1);
        modelState["OtherRouteIntoSocialWork"]!.Errors[0].ErrorMessage.Should()
            .Be("Enter your entry route");

        Sut.BackLinkPath.Should().Be("/social-worker-registration/select-social-work-qualification-end-year");

        VerifyAllNoOtherCalls();
    }
}
