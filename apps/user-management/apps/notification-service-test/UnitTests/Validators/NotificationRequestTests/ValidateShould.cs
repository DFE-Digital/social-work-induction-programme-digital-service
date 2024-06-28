using DfeSwwEcf.NotificationService.Models;
using DfeSwwEcf.NotificationService.Validation;
using FluentValidation.TestHelper;

namespace DfeSwwEcf.NotificationService.Tests.UnitTests.Validators.NotificationRequestTests;

public class ValidateShould
{
    private NotificationRequestValidator _validator;

    public ValidateShould()
    {
        _validator = new();
    }

    [Fact]
    public void WhenEmailAndTemplateAreSupplied_PassesValidation()
    {
        // Arrange
        var model = new NotificationRequest
        {
            EmailAddress = "test@test.com",
            TemplateId = Guid.NewGuid()
        };

        //Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void WhenEmailAndTemplateNotSupplied_HasValidationErrors()
    {
        // Arrange
        var model = new NotificationRequest
        {
            EmailAddress = "NOT AN EMAIL",
            TemplateId = Guid.Empty
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(person => person.EmailAddress);
        result.ShouldHaveValidationErrorFor(person => person.TemplateId);
    }
}
