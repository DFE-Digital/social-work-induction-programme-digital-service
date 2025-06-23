using System.ComponentModel.DataAnnotations;
using Dfe.Sww.Ecf.Frontend.Pages.SocialWorkerRegistration;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Validation.RegisterSocialWorker;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using NodaTime;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.SocialWorkerRegistration;

public class SelectDateOfBirthPageTests : SocialWorkerRegistrationPageTestBase
{
    private SelectDateOfBirth Sut { get; }

    public SelectDateOfBirthPageTests()
    {
        Sut = new(new FakeLinkGenerator(), MockRegisterSocialWorkerJourneyService.Object, MockAuthServiceClient.Object,
            new SocialWorkerDateOfBirthValidator());
    }

    [Fact]
    public async Task OnGetAsync_WhenCalled_LoadsTheView()
    {
        // Arrange
        MockAuthServiceClient.Setup(x => x.HttpContextService.GetPersonId()).Returns(PersonId);

        var dob = DateOnly.FromDateTime(DateTime.UtcNow);
        var expectedDob = LocalDate.FromDateOnly(dob);
        MockRegisterSocialWorkerJourneyService.Setup(x => x.GetDateOfBirthAsync(PersonId)).ReturnsAsync(dob);

        // Act
        var result = await Sut.OnGetAsync();

        // Assert
        Sut.UserDateOfBirth.Should().Be(expectedDob);
        Sut.BackLinkPath.Should().Be("/social-worker-registration");
        result.Should().BeOfType<PageResult>();

        MockAuthServiceClient.Verify(x => x.HttpContextService.GetPersonId(), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.GetDateOfBirthAsync(PersonId), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithValidDate_SavesDateAndRedirectsUser()
    {
        // Arrange
        MockAuthServiceClient.Setup(x => x.HttpContextService.GetPersonId()).Returns(PersonId);

        Sut.UserDateOfBirth = new LocalDate(2020, 12, 31);
        var expectedDob = new DateOnly(
            Sut.UserDateOfBirth.Value.Year,
            Sut.UserDateOfBirth.Value.Month,
            Sut.UserDateOfBirth.Value.Day);

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("/social-worker-registration/select-sex-and-gender-identity");

        MockAuthServiceClient.Verify(x => x.HttpContextService.GetPersonId(), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.SetDateOfBirthAsync(PersonId, expectedDob), Times.Once);

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithInvalidNullDate_ReturnsValidationErrors()
    {
        // Arrange
        Sut.UserDateOfBirth = null;

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

        modelStateKeys.Should().Contain("UserDateOfBirth");
        modelState["UserDateOfBirth"]!.Errors.Count.Should().Be(1);
        modelState["UserDateOfBirth"]!.Errors[0].ErrorMessage.Should()
            .Be("Enter your date of birth");

        Sut.BackLinkPath.Should().Be("/social-worker-registration");

        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithInvalidFutureDate_ReturnsValidationErrors()
    {
        // Arrange
        Sut.UserDateOfBirth = new LocalDate(2100, 12, 31);

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<PageResult>();

        var modelState = Sut.ModelState;
        var modelStateKeys = modelState.Keys.ToList();
        modelStateKeys.Count.Should().Be(1);

        modelStateKeys.Should().Contain("UserDateOfBirth");
        modelState["UserDateOfBirth"]!.Errors.Count.Should().Be(1);
        modelState["UserDateOfBirth"]!.Errors[0].ErrorMessage.Should()
            .Be("Date of birth must be in the past");

        Sut.BackLinkPath.Should().Be("/social-worker-registration");

        VerifyAllNoOtherCalls();
    }
}
