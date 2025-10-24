using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using Dfe.Sww.Ecf.Frontend.Validation;
using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Validators;

public class AccountDetailsValidatorTests()
    : ValidatorTestBase<AccountDetailsValidator, AccountDetails, AccountDetailsFaker>(
        new AccountDetailsValidator(new Mock<IAccountService>().Object),
        new AccountDetailsFaker()
    )
{
    [Fact]
    public async Task WhenRequiredPropertiesAreSupplied_PassesValidation()
    {
        // Arrange
        var newAccount = Faker.Generate();

        //Act
        var result = await Sut.TestValidateAsync(newAccount);

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
    public async Task WhenSocialWorkerNumberIsInvalid_HasValidationErrors(string? sweId)
    {
        // Arrange
        var account = Faker.GenerateWithSweIdAndRelevantAccountType(sweId, ImmutableList.Create(AccountType.EarlyCareerSocialWorker));

        // Act
        var result = await Sut.TestValidateAsync(account);

        // Assert
        result
            .ShouldHaveValidationErrorFor(person => person.SocialWorkEnglandNumber)
            .WithErrorMessage("Enter a Social Work England registration number in the correct format");
    }

    [Fact]
    public void WhenEmailIsNotInEmailFormat_HaveValidationErrors()
    {
        // Arrange
        var account = Faker.GenerateWithInvalidEmail("NotAnEmail:Oops.com");

        // Act
        var result = Sut.TestValidate(account);

        // Assert
        result.ShouldHaveValidationErrorFor(person => person.Email).WithErrorMessage("Enter an email address in the correct format, like name@example.com");
    }

    [Fact]
    public async Task WhenEmailAlreadyExists_HasValidationError()
    {
        // Arrange
        var account = new AccountDetailsFaker().Generate();
        var accountService = new Mock<IAccountService>();
        accountService.Setup(s => s.CheckEmailExistsAsync(account.Email!)).ReturnsAsync(true);

        var validator = new AccountDetailsValidator(accountService.Object);

        // Act
        var result = await validator.TestValidateAsync(account);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorMessage("The email address entered belongs to an existing user");
    }

    [Fact]
    public async Task WhenEmailIsUnique_WhenRootContextFlagFalse_PassesValidation()
    {
        // Arrange
        var account = new AccountDetailsFaker().Generate();
        var accountService = new Mock<IAccountService>();
        accountService.Setup(s => s.CheckEmailExistsAsync(account.Email!)).ReturnsAsync(false);

        var validator = new AccountDetailsValidator(accountService.Object);
        var rootContext = new ValidationContext<AccountDetails>(account)
        {
            RootContextData =
            {
                ["SkipEmailUnique"] = false
            }
        };

        // Act
        var result = await validator.TestValidateAsync(rootContext);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
        accountService.Verify<Task<bool>>(x => x.CheckEmailExistsAsync(It.IsAny<string>()), Times.Once);

    }

    [Fact]
    public async Task WhenRootContextFlagTrue_EmailUniquenessCheckSkipped_AndPassesValidation()
    {
        // Arrange
        var account = new AccountDetailsFaker().Generate();
        var accountService = new Mock<IAccountService>();
        var validator = new AccountDetailsValidator(accountService.Object);

        var rootContext = new ValidationContext<AccountDetails>(account)
        {
            RootContextData =
            {
                ["SkipEmailUnique"] = true
            }
        };

        // Act
        var result = await validator.ValidateAsync(rootContext);

        // Assert
        result.IsValid.Should().BeTrue();
        accountService.Verify<Task<bool>>(x => x.CheckEmailExistsAsync(It.IsAny<string>()), Times.Never);
    }
}
