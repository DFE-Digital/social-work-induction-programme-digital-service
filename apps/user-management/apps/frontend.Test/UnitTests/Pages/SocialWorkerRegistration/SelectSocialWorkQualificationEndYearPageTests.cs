using System.ComponentModel.DataAnnotations;
using Dfe.Sww.Ecf.Frontend.Pages.SocialWorkerRegistration;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Validation;
using Dfe.Sww.Ecf.Frontend.Validation.RegisterSocialWorker;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using NodaTime;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.SocialWorkerRegistration;

public class SelectSocialWorkerRegistrationEndYearPageTests : SocialWorkerRegistrationPageTestBase
{
    private SelectSocialWorkQualificationEndYear Sut { get; }

    public SelectSocialWorkerRegistrationEndYearPageTests()
    {
        Sut = new(new FakeLinkGenerator(), MockRegisterSocialWorkerJourneyService.Object, MockAuthServiceClient.Object,
            new SocialWorkQualificationEndYearValidator());
    }

    [Fact]
    public async Task OnGetAsync_WhenCalled_LoadsTheView()
    {
        // Arrange
        MockAuthServiceClient.Setup(x => x.HttpContextService.GetPersonId()).Returns(PersonId);

        var expectedRegYear = 2020;
        MockRegisterSocialWorkerJourneyService.Setup(x => x.GetSocialWorkQualificationEndYearAsync(PersonId)).ReturnsAsync(expectedRegYear);

        // Act
        var result = await Sut.OnGetAsync();

        // Assert
        Sut.SocialWorkQualificationEndYear.Should().Be(expectedRegYear);
        Sut.BackLinkPath.Should().Be("/social-worker-registration/select-highest-qualification");
        result.Should().BeOfType<PageResult>();

        MockAuthServiceClient.Verify(x => x.HttpContextService.GetPersonId(), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.GetSocialWorkQualificationEndYearAsync(PersonId), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithValidDate_SavesDateAndRedirectsUser()
    {
        // Arrange
        MockAuthServiceClient.Setup(x => x.HttpContextService.GetPersonId()).Returns(PersonId);

        var expectedEndYear = 2020;
        Sut.SocialWorkQualificationEndYear = expectedEndYear;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/social-worker-registration/select-route-into-social-work");

        MockAuthServiceClient.Verify(x => x.HttpContextService.GetPersonId(), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.SetSocialWorkQualificationEndYearAsync(PersonId, expectedEndYear), Times.Once);

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithInvalidNullDate_ReturnsValidationErrors()
    {
        // Arrange
        Sut.SocialWorkQualificationEndYear = null;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);

        modelStateKeys.Should().Contain("SocialWorkQualificationEndYear");
        modelState["SocialWorkQualificationEndYear"]!.Errors.Count.Should().Be(1);
        modelState["SocialWorkQualificationEndYear"]!.Errors[0].ErrorMessage.Should()
            .Be("Enter the year you finished your social work qualification");

        Sut.BackLinkPath.Should().Be("/social-worker-registration/select-highest-qualification");

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithInvalidFutureDate_ReturnsValidationErrors()
    {
        // Arrange
        Sut.SocialWorkQualificationEndYear = 2200;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);

        modelStateKeys.Should().Contain("SocialWorkQualificationEndYear");
        modelState["SocialWorkQualificationEndYear"]!.Errors.Count.Should().Be(1);
        modelState["SocialWorkQualificationEndYear"]!.Errors[0].ErrorMessage.Should()
            .Be("The year you finished your social work qualification must be in the past");

        Sut.BackLinkPath.Should().Be("/social-worker-registration/select-highest-qualification");

        VerifyAllNoOtherCalls();
    }

    [Theory]
    [InlineData(123)]
    [InlineData(-123)]
    public async Task OnPostAsync_WhenCalledWithInvalidYearFormat_ReturnsValidationErrors(int year)
    {
        // Arrange
        Sut.SocialWorkQualificationEndYear = year;

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);

        modelStateKeys.Should().Contain("SocialWorkQualificationEndYear");
        modelState["SocialWorkQualificationEndYear"]!.Errors.Count.Should().Be(1);
        modelState["SocialWorkQualificationEndYear"]!.Errors[0].ErrorMessage.Should()
            .Be("Enter a real year");

        Sut.BackLinkPath.Should().Be("/social-worker-registration/select-highest-qualification");

        VerifyAllNoOtherCalls();
    }
}
