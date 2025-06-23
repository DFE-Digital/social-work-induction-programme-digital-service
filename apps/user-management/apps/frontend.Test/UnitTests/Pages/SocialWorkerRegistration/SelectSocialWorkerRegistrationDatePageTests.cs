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

public class SelectSocialWorkerRegistrationDatePageTests : SocialWorkerRegistrationPageTestBase
{
    private SelectSocialWorkEnglandRegistrationDate Sut { get; }

    public SelectSocialWorkerRegistrationDatePageTests()
    {
        Sut = new(new FakeLinkGenerator(), MockRegisterSocialWorkerJourneyService.Object, MockAuthServiceClient.Object,
            new SocialWorkerSocialWorkEnglandRegistrationDateValidator());
    }

    [Fact]
    public async Task OnGetAsync_WhenCalled_LoadsTheView()
    {
        // Arrange
        MockAuthServiceClient.Setup(x => x.HttpContextService.GetPersonId()).Returns(PersonId);

        var regDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var expectedRegDate = LocalDate.FromDateOnly(regDate);
        MockRegisterSocialWorkerJourneyService.Setup(x => x.GetSocialWorkEnglandRegistrationDateAsync(PersonId)).ReturnsAsync(regDate);

        // Act
        var result = await Sut.OnGetAsync();

        // Assert
        Sut.SocialWorkEnglandRegistrationDate.Should().Be(expectedRegDate);
        Sut.BackLinkPath.Should().Be("/social-worker-registration/select-disability");
        result.Should().BeOfType<PageResult>();

        MockAuthServiceClient.Verify(x => x.HttpContextService.GetPersonId(), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.GetSocialWorkEnglandRegistrationDateAsync(PersonId), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithValidDate_SavesDateAndRedirectsUser()
    {
        // Arrange
        MockAuthServiceClient.Setup(x => x.HttpContextService.GetPersonId()).Returns(PersonId);

        Sut.SocialWorkEnglandRegistrationDate = new LocalDate(2020, 12, 31);
        var expectedRegDate = new DateOnly(
            Sut.SocialWorkEnglandRegistrationDate.Value.Year,
            Sut.SocialWorkEnglandRegistrationDate.Value.Month,
            Sut.SocialWorkEnglandRegistrationDate.Value.Day);

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/social-worker-registration/select-highest-qualification");

        MockAuthServiceClient.Verify(x => x.HttpContextService.GetPersonId(), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.SetSocialWorkEnglandRegistrationDateAsync(PersonId, expectedRegDate), Times.Once);

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithInvalidNullDate_ReturnsValidationErrors()
    {
        // Arrange
        Sut.SocialWorkEnglandRegistrationDate = null;

        // Simulate non-fluent validation model validation
        var validationContext = new ValidationContext(Sut, null, null);
        var validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(Sut, validationContext, validationResults, true);

        foreach (var validationResult in validationResults)
        {
            foreach (var memberName in validationResult.MemberNames)
            {
                Sut.ModelState.AddModelError(memberName, validationResult.ErrorMessage!);
            }
        }

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);

        modelStateKeys.Should().Contain("SocialWorkEnglandRegistrationDate");
        modelState["SocialWorkEnglandRegistrationDate"]!.Errors.Count.Should().Be(1);
        modelState["SocialWorkEnglandRegistrationDate"]!.Errors[0].ErrorMessage.Should()
            .Be("Enter the date you were added to the Social Work England register");

        Sut.BackLinkPath.Should().Be("/social-worker-registration/select-disability");

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithInvalidFutureDate_ReturnsValidationErrors()
    {
        // Arrange
        Sut.SocialWorkEnglandRegistrationDate = new LocalDate(2100, 12, 31);

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);

        modelStateKeys.Should().Contain("SocialWorkEnglandRegistrationDate");
        modelState["SocialWorkEnglandRegistrationDate"]!.Errors.Count.Should().Be(1);
        modelState["SocialWorkEnglandRegistrationDate"]!.Errors[0].ErrorMessage.Should()
            .Be("Date were you added to the Social Work England register must be in the past");

        Sut.BackLinkPath.Should().Be("/social-worker-registration/select-disability");

        VerifyAllNoOtherCalls();
    }
}
