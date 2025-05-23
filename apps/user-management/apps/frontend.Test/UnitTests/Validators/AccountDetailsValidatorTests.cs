using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using Dfe.Sww.Ecf.Frontend.Validation;
using FluentValidation.TestHelper;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Validators;

public class AccountDetailsValidatorTests()
    : ValidatorTestBase<AccountDetailsValidator, AccountDetails, AccountDetailsFaker>(
        new AccountDetailsValidator(),
        new AccountDetailsFaker()
    )
{
    [Fact]
    public void WhenRequiredPropertiesAreSupplied_PassesValidation()
    {
        // Arrange
        var newAccount = Faker.Generate();

        //Act
        var result = Sut.TestValidate(newAccount);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("     ")]
    public void WhenAnyInputNotSupplied_WhenIsNotStaff_HasValidationErrors(string? value)
    {
        // Arrange
        var account = new AccountDetails
        {
            IsStaff = false,
            FirstName = value,
            LastName = value,
            Email = value,
            SocialWorkEnglandNumber = value
        };

        // Act
        var result = Sut.TestValidate(account);

        // Assert
        result
            .ShouldHaveValidationErrorFor(person => person.FirstName)
            .WithErrorMessage("Enter a first name");
        result
            .ShouldHaveValidationErrorFor(person => person.LastName)
            .WithErrorMessage("Enter a last name");
        result
            .ShouldHaveValidationErrorFor(person => person.Email)
            .WithErrorMessage("Enter an email address");
        result
            .ShouldHaveValidationErrorFor(person => person.SocialWorkEnglandNumber)
            .WithErrorMessage("Enter a Social Work England registration number");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("     ")]
    public void WhenAnyInputNotSupplied_WhenIsStaff_HasValidationErrors(string? value)
    {
        // Arrange
        var account = new AccountDetails
        {
            IsStaff = true,
            FirstName = value,
            LastName = value,
            Email = value
        };

        // Act
        var result = Sut.TestValidate(account);

        // Assert
        result
            .ShouldHaveValidationErrorFor(person => person.FirstName)
            .WithErrorMessage("Enter a first name");
        result
            .ShouldHaveValidationErrorFor(person => person.LastName)
            .WithErrorMessage("Enter a last name");
        result
            .ShouldHaveValidationErrorFor(person => person.Email)
            .WithErrorMessage("Enter an email address");
    }

    [Theory]
    [InlineData("SWE123")]
    [InlineData("SW123456789123456789")]
    [InlineData("123456789123456789")]
    [InlineData("-500")]
    [InlineData("0")]
    public void WhenSocialWorkerNumberIsInvalid_HasValidationErrors(string? sweId)
    {
        // Arrange
        var account = Faker.GenerateWithSweId(sweId);

        // Act
        var result = Sut.TestValidate(account);

        // Assert
        result
            .ShouldHaveValidationErrorFor(person => person.SocialWorkEnglandNumber)
            .WithErrorMessage("Social Work England Number is in an invalid format");
    }

    [Fact]
    public void WhenEmailIsNotInEmailFormat_HaveValidationErrors()
    {
        // Arrange
        var account = Faker.GenerateWithInvalidEmail("NotAnEmail:Oops.com");

        // Act
        var result = Sut.TestValidate(account);

        // Assert
        result
            .ShouldHaveValidationErrorFor(person => person.Email)
            .WithErrorMessage(
                "Enter an email address in the correct format, like name@example.com"
            );
    }
}
