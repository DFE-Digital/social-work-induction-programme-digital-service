using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using Dfe.Sww.Ecf.Frontend.Validation;
using FluentValidation.TestHelper;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Validators.AccountDetailsTests;

public class ValidateShould()
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
    [InlineData("John", null)]
    [InlineData(null, "Doe")]
    public void WhenAtLeastOneNameIsSupplied_PassesValidation(string? firstName, string? lastName)
    {
        // Arrange
        var newAccount = Faker.GenerateWithCustomName(firstName, lastName);

        //Act
        var result = Sut.TestValidate(newAccount);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("     ")]
    public void WhenEmailAndFirstNameAndLastNameAreNotSupplied_HasValidationErrors(string? value)
    {
        // Arrange
        var account = new AccountDetails
        {
            FirstName = value,
            LastName = value,
            Email = value
        };

        // Act
        var result = Sut.TestValidate(account);

        // Assert
        result
            .ShouldHaveValidationErrorFor(person => person.FirstName)
            .WithErrorMessage("Enter a first or last name");
        result
            .ShouldHaveValidationErrorFor(person => person.LastName)
            .WithErrorMessage("Enter a first or last name");
        result
            .ShouldHaveValidationErrorFor(person => person.Email)
            .WithErrorMessage("Enter an email");
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
