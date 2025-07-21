using System.Collections.Immutable;
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
    public void WhenAnyInputNotSupplied_WhenIsSocialWorkerOrAssessor_HasValidationErrors(string? value)
    {
        // Arrange
        var account = new AccountDetails
        {
            Types = ImmutableList.Create(AccountType.EarlyCareerSocialWorker),
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
    public void WhenAnyInputNotSupplied_WhenIsCoordinator_HasValidationErrors(string? value)
    {
        // Arrange
        var account = new AccountDetails
        {
            Types = ImmutableList.Create(AccountType.Coordinator),
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
        var account = Faker.GenerateWithSweIdAndRelevantAccountType(sweId,ImmutableList.Create(AccountType.EarlyCareerSocialWorker));

        // Act
        var result = Sut.TestValidate(account);

        // Assert
        result
            .ShouldHaveValidationErrorFor(person => person.SocialWorkEnglandNumber)
            .WithErrorMessage("Enter a Social Work England registration number in the correct format, like SW1234");
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

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("     ")]
    public void WhenAnyInputNotSupplied_WhenRequiresPhoneNumber_HasValidationErrors(string? value)
    {
        // Arrange
        var account = new AccountDetails
        {
            FirstName = value,
            LastName = value,
            Email = value,
            PhoneNumber = value,
            PhoneNumberRequired = true
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
            .ShouldHaveValidationErrorFor(person => person.PhoneNumber)
            .WithErrorMessage("Enter a phone number, like 01632 960 001, 07700 900 982 or +44 808 157 0192");
    }

    [Theory]
    [InlineData("ggfgffgf")]
    [InlineData("+451234123412")]
    [InlineData("00174123123")]
    [InlineData("075742367455")]
    [InlineData("+44074238521312")]
    [InlineData("+4401275234234")]
    public void WhenPhoneNumberIsInvalidFormat_HasValidationErrors(string? phoneNumber)
    {
        // Arrange
        var account = Faker.GenerateWithPhoneNumber(phoneNumber);

        // Act
        var result = Sut.TestValidate(account);

        // Assert
        result
            .ShouldHaveValidationErrorFor(person => person.PhoneNumber)
            .WithErrorMessage("Enter a phone number, like 01632 960 001, 07700 900 982 or +44 808 157 0192");
    }

    [Theory]
    [InlineData("01253 123 123")]
    [InlineData("01253123123")]
    [InlineData("01253 12 31 23")]
    [InlineData("+447650123123")]
    [InlineData("+447650 123 123")]
    [InlineData("+447650 12 31 23")]
    [InlineData("07542123543")]
    [InlineData("07542 123 543")]
    [InlineData("07542 12 35 43")]
    [InlineData("+441274 123 123")]
    [InlineData("+441274 12 31 23")]
    public void WhenPhoneNumberInValidFormat_HasNoValidationErrors(string? phoneNumber)
    {
        // Arrange
        var account = Faker.GenerateWithPhoneNumber(phoneNumber);

        // Act
        var result = Sut.TestValidate(account);

        // Assert
        result
            .ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
    }
}
