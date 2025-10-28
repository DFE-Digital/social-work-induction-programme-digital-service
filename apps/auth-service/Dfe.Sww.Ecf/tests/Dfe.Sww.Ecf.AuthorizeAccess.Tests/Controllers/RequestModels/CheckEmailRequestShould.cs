using System.ComponentModel.DataAnnotations;
using Dfe.Sww.Ecf.AuthorizeAccess.Controllers.Accounts;
using FluentAssertions;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Tests.Controllers.RequestModels;

public class CheckEmailRequestShould
{
    [Fact]
    public void FailValidation_WhenEmailMissing()
    {
        // Arrange
        var request = new CheckEmailRequest { Email = string.Empty };
        var validationResults = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(request, new ValidationContext(request), validationResults, validateAllProperties: true);

        // Assert
        isValid.Should().BeFalse();
        validationResults.Should().ContainSingle(r =>
            r.MemberNames.Contains(nameof(CheckEmailRequest.Email)) &&
            r.ErrorMessage == "Email is required.");
    }

    [Fact]
    public void FailsValidation_WhenEmailInvalidFormat()
    {
        // Arrange
        var request = new CheckEmailRequest { Email = "invalid email" };
        var validationResults = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(request, new ValidationContext(request), validationResults, validateAllProperties: true);

        // Assert
        isValid.Should().BeFalse();
        validationResults.Should().ContainSingle(r =>
            r.MemberNames.Contains(nameof(CheckEmailRequest.Email)) &&
            r.ErrorMessage == "Email is not a valid format.");
    }
}
