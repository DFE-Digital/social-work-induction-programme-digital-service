using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Services;
using Dfe.Sww.Ecf.Frontend.Validation;
using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using Moq;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Validators;

public class AccountDetailsValidatorTests()
    : ValidatorTestBase<AccountDetailsValidator, AccountDetails, AccountDetailsFaker>(
        new AccountDetailsValidator(
            (_mockAccountService = new Mock<IAccountService>()).Object,
            (_mockAuthServiceClient = new Mock<IAuthServiceClient>()).Object),
        new AccountDetailsFaker())
{
    private static Mock<IAccountService> _mockAccountService = default!;
    private static Mock<IAuthServiceClient> _mockAuthServiceClient = default!;

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
    public async Task WhenAnyInputNotSupplied_WhenIsSocialWorkerOrAssessor_HasValidationErrors(string? value)
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
        var result = await Sut.TestValidateAsync(account);

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

        _mockAuthServiceClient.Setup(x => x.Accounts.GetBySocialWorkEnglandNumberAsync(account.SocialWorkEnglandNumber!)).ReturnsAsync((Person?)null);

        var rootContext = new ValidationContext<AccountDetails>(account)
        {
            RootContextData =
            {
                ["SkipSweIdUnique"] = false
            }
        };

        // Act
        var result = await Sut.TestValidateAsync(rootContext);

        // Assert
        result
            .ShouldHaveValidationErrorFor(person => person.SocialWorkEnglandNumber)
            .WithErrorMessage("Enter a Social Work England registration number in the correct format");

        _mockAccountService.Verify<Task<bool>>(x => x.CheckEmailExistsAsync(account.Email!), Times.Once);
        _mockAuthServiceClient.Verify(x => x.Accounts.GetBySocialWorkEnglandNumberAsync(account.SocialWorkEnglandNumber!), Times.Once);
        VerifyAllNoOtherCall();
    }

    [Theory]
    [InlineData(AccountType.EarlyCareerSocialWorker)]
    [InlineData(AccountType.Assessor)]
    public async Task WhenSocialWorkEnglandNumberIsTaken_HaveValidationErrors(AccountType accountType)
    {
        // Arrange
        var account = Faker.GenerateWithSweIdAndRelevantAccountType("SW123", new List<AccountType> { accountType });
        var person = new Person
        {
            PersonId = account.Id,
            CreatedOn = DateTime.UtcNow,
            FirstName = account.FirstName!,
            LastName = account.LastName!
        };

        _mockAuthServiceClient.Setup(x => x.Accounts.GetBySocialWorkEnglandNumberAsync(account.SocialWorkEnglandNumber!)).ReturnsAsync(person);

        var rootContext = new ValidationContext<AccountDetails>(account)
        {
            RootContextData =
            {
                ["SkipSweIdUnique"] = false
            }
        };

        // Act
        var result = await Sut.TestValidateAsync(rootContext);

        // Assert
        result.ShouldHaveValidationErrorFor(validationPerson => validationPerson.SocialWorkEnglandNumber)
            .WithErrorMessage("The Social Work England registration number entered belongs to an existing user");

        _mockAccountService.Verify<Task<bool>>(x => x.CheckEmailExistsAsync(account.Email!), Times.Once);
        _mockAuthServiceClient.Verify(x => x.Accounts.GetBySocialWorkEnglandNumberAsync(account.SocialWorkEnglandNumber!), Times.Once);
        VerifyAllNoOtherCall();
    }

    [Theory]
    [InlineData(AccountType.EarlyCareerSocialWorker)]
    [InlineData(AccountType.Assessor)]
    public async Task WhenSocialWorkEnglandNumberIsNotTaken_HaveValidationErrors(AccountType accountType)
    {
        // Arrange
        var account = Faker.GenerateWithSweIdAndRelevantAccountType("SW123", new List<AccountType> { accountType });

        _mockAuthServiceClient.Setup(x => x.Accounts.GetBySocialWorkEnglandNumberAsync(account.SocialWorkEnglandNumber!)).ReturnsAsync((Person?)null);

        var rootContext = new ValidationContext<AccountDetails>(account)
        {
            RootContextData =
            {
                ["SkipSweIdUnique"] = false
            }
        };

        // Act
        var result = await Sut.TestValidateAsync(rootContext);

        // Assert
        result.ShouldNotHaveValidationErrorFor(validationPerson => validationPerson.SocialWorkEnglandNumber);

        _mockAccountService.Verify<Task<bool>>(x => x.CheckEmailExistsAsync(account.Email!), Times.Once);
        _mockAuthServiceClient.Verify(x => x.Accounts.GetBySocialWorkEnglandNumberAsync(account.SocialWorkEnglandNumber!), Times.Once);
        VerifyAllNoOtherCall();
    }

    [Fact]
    public async Task WhenRunForCoordinatorAccountType_NotHaveValidationErrors()
    {
        // Arrange
        var account = Faker.GenerateWithSweIdAndRelevantAccountType("SW123", new List<AccountType> { AccountType.Coordinator });

        // Act
        var result = await Sut.TestValidateAsync(account);

        // Assert
        result.ShouldNotHaveValidationErrorFor(validationPerson => validationPerson.SocialWorkEnglandNumber);

        _mockAccountService.Verify(x => x.CheckEmailExistsAsync(account.Email!), Times.Once);
        _mockAuthServiceClient.Verify(x => x.Accounts.GetBySocialWorkEnglandNumberAsync(account.SocialWorkEnglandNumber!), Times.Never);
        VerifyAllNoOtherCall();
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

        VerifyAllNoOtherCall();
    }

    [Fact]
    public async Task WhenEmailAlreadyExists_HasValidationError()
    {
        // Arrange
        var account = new AccountDetailsFaker().Generate();

        _mockAccountService.Setup(x => x.CheckEmailExistsAsync(account.Email!)).ReturnsAsync(true);

        // Act
        var result = await Sut.TestValidateAsync(account);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorMessage("The email address entered belongs to an existing user");

        _mockAccountService.Verify(x => x.CheckEmailExistsAsync(account.Email!), Times.Once);
        VerifyAllNoOtherCall();
    }

    [Fact]
    public async Task WhenEmailIsUnique_WhenRootContextFlagFalse_PassesValidation()
    {
        // Arrange
        var account = new AccountDetailsFaker().Generate();

        var rootContext = new ValidationContext<AccountDetails>(account)
        {
            RootContextData =
            {
                ["SkipEmailUnique"] = false
            }
        };

        _mockAccountService.Setup(x => x.CheckEmailExistsAsync(account.Email!)).ReturnsAsync(false);

        // Act
        var result = await Sut.TestValidateAsync(rootContext);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
        _mockAccountService.Verify<Task<bool>>(x => x.CheckEmailExistsAsync(It.IsAny<string>()), Times.Once);
        VerifyAllNoOtherCall();
    }

    [Fact]
    public async Task WhenEmailRootContextFlagTrue_EmailUniquenessCheckSkipped_AndPassesValidation()
    {
        // Arrange
        var account = new AccountDetailsFaker().Generate();

        var rootContext = new ValidationContext<AccountDetails>(account)
        {
            RootContextData =
            {
                ["SkipEmailUnique"] = true
            }
        };

        // Act
        var result = await Sut.ValidateAsync(rootContext);

        // Assert
        result.IsValid.Should().BeTrue();
        _mockAccountService.Verify<Task<bool>>(x => x.CheckEmailExistsAsync(It.IsAny<string>()), Times.Never);
        VerifyAllNoOtherCall();
    }

    [Fact]
    public async Task WhenSweIdIsUnique_WhenRootContextFlagFalse_PassesValidation()
    {
        // Arrange
        var account = new AccountDetailsFaker().Generate();

        var rootContext = new ValidationContext<AccountDetails>(account)
        {
            RootContextData =
            {
                ["SkipSweIdUnique"] = false
            }
        };

        _mockAuthServiceClient.Setup(x => x.Accounts.GetBySocialWorkEnglandNumberAsync(account.Email!)).ReturnsAsync((Person?)null);

        // Act
        var result = await Sut.TestValidateAsync(rootContext);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Email);

        _mockAccountService.Verify<Task<bool>>(x => x.CheckEmailExistsAsync(It.IsAny<string>()), Times.Once);
        VerifyAllNoOtherCall();
    }

    [Fact]
    public async Task WhenSweIdRootContextFlagTrue_SweIdUniquenessCheckSkipped_AndPassesValidation()
    {
        // Arrange
        var account = new AccountDetailsFaker().Generate();

        var rootContext = new ValidationContext<AccountDetails>(account)
        {
            RootContextData =
            {
                ["SkipSweIdUnique"] = true
            }
        };

        // Act
        var result = await Sut.ValidateAsync(rootContext);

        // Assert
        result.IsValid.Should().BeTrue();
        _mockAccountService.Verify<Task<bool>>(x => x.CheckEmailExistsAsync(account.Email!), Times.Once);
        _mockAuthServiceClient.Verify(x => x.Accounts.GetBySocialWorkEnglandNumberAsync(It.IsAny<string>()), Times.Never);
        VerifyAllNoOtherCall();
    }

    private void VerifyAllNoOtherCall()
    {
        _mockAccountService.VerifyNoOtherCalls();
        _mockAuthServiceClient.VerifyNoOtherCalls();
    }
}
