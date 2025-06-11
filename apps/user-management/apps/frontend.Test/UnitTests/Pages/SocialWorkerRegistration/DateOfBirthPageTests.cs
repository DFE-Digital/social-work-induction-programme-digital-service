using System.ComponentModel.DataAnnotations;
using Dfe.Sww.Ecf.Frontend.Pages.SocialWorkerRegistration;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Validation;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using NodaTime;
using Xunit;
using SocialWorkerRegistrationIndex = Dfe.Sww.Ecf.Frontend.Pages.SocialWorkerRegistration.Index;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Pages.SocialWorkerRegistration;

public class DateOfBirthPageTests : SocialWorkerRegistrationPageTestBase<SocialWorkerRegistrationIndex>
{
    private DateOfBirth Sut { get; }

    public DateOfBirthPageTests()
    {
        Sut = new(new FakeLinkGenerator(), MockRegisterSocialWorkerJourneyService.Object, MockAuthServiceClient.Object,
            new SocialWorkerDateOfBirthValidator());
    }

    [Fact]
    public async Task OnGetAsync_WhenCalled_LoadsTheView()
    {
        // Arrange
        var personId = Guid.NewGuid();
        MockAuthServiceClient.SetupMockHttpContextAccessorWithPersonId(personId);
        MockAuthServiceClient.Setup(x => x.HttpContextService.GetPersonId()).Returns(personId);

        var dob = DateTime.UtcNow;
        var expectedDob = LocalDate.FromDateTime(dob);
        MockRegisterSocialWorkerJourneyService.Setup(x => x.GetDateOfBirthAsync(personId)).ReturnsAsync(dob);

        // Act
        var result = await Sut.OnGetAsync();

        // Assert
        Sut.UserDateOfBirth.Should().Be(expectedDob);
        Sut.BackLinkPath.Should().Be("/social-worker-registration");
        result.Should().BeOfType<PageResult>();

        MockAuthServiceClient.Verify(x => x.HttpContextService.GetPersonId(), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.GetDateOfBirthAsync(personId), Times.Once);
        VerifyAllNoOtherCalls();
    }

    [Fact]
    public async Task OnPostAsync_WhenCalledWithValidDate_SavesDateAndRedirectsUser()
    {
        // Arrange
        var personId = Guid.NewGuid();
        MockAuthServiceClient.SetupMockHttpContextAccessorWithPersonId(personId);
        MockAuthServiceClient.Setup(x => x.HttpContextService.GetPersonId()).Returns(personId);

        Sut.UserDateOfBirth = new LocalDate(2020, 12, 31);
        var expectedDob = new DateTime(
            Sut.UserDateOfBirth.Value.Year,
            Sut.UserDateOfBirth.Value.Month,
            Sut.UserDateOfBirth.Value.Day);

        // Act
        var result = await Sut.OnPostAsync();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("index"); // TODO update this ECSW sex and gender page

        MockAuthServiceClient.Verify(x => x.HttpContextService.GetPersonId(), Times.Once);
        MockRegisterSocialWorkerJourneyService.Verify(x => x.SetDateOfBirthAsync(personId, expectedDob), Times.Once);

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
