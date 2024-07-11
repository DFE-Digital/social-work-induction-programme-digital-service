using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Validation;
using FluentValidation.TestHelper;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Validators.AccountTests;

public class ValidateShould
{
    private readonly AccountValidator _validator;

    public ValidateShould()
    {
        _validator = new AccountValidator();
    }

    [Fact]
    public void WhenRequiredPropertiesAreSupplied_PassesValidation()
    {
        // Arrange
        var account = new Account
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "John.Doe@test.com"
        };

        //Act
        var result = _validator.TestValidate(account);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void WhenEmailAndFirstNameAndLastNameAreNotSupplied_HasValidationErrors()
    {
        // Arrange
        var account = new Account
        {
            FirstName = string.Empty,
            LastName = string.Empty,
            Email = string.Empty
        };

        // Act
        var result = _validator.TestValidate(account);

        // Assert
        result.ShouldHaveValidationErrorFor(person => person.FirstName);
        result.ShouldHaveValidationErrorFor(person => person.LastName);
        result.ShouldHaveValidationErrorFor(person => person.Email);
    }
}
