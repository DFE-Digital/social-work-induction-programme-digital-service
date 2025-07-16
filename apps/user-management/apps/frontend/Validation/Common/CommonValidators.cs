using System.Text.RegularExpressions;
using Dfe.Sww.Ecf.Frontend.Models;
using FluentValidation;
using FluentValidation.Results;
using NodaTime;

namespace Dfe.Sww.Ecf.Frontend.Validation.Common;

public static class CommonValidators
{
    public static void FirstNameValidation<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        ruleBuilder.NotEmpty().WithMessage("Enter a first name");
    }

    public static void LastNameValidation<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        ruleBuilder.NotEmpty().WithMessage("Enter a last name");
    }

    public static void EmailValidation<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        ruleBuilder
            .NotEmpty()
            .WithMessage("Enter an email address")
            .EmailAddress()
            .WithMessage("Enter an email address in the correct format, like name@example.com");
    }

    public static void PhoneNumberValidation<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        const string errorMessage = "Enter a phone number, like 01632 960 001, 07700 900 982 or +44 808 157 0192";

        const string internationalMobileRegex = @"^\+44\s?7\d{3}\s?\d{3}\s?\d{3}$";

        const string mobileRegex = @"^07\d{3}\s?\d{3}\s?\d{3}$";

        const string landlineRegex = @"^0[12]\d{1,3}\s?\d{3}\s?\d{3,4}$";

        ruleBuilder
            .NotEmpty().WithMessage(errorMessage)
            .Must(phoneNumber =>
                phoneNumber is not null && new[] { internationalMobileRegex, mobileRegex, landlineRegex }
                    .Any(pattern => Regex.IsMatch(phoneNumber, pattern))
            )
            .WithMessage(errorMessage);
    }

    public static void PastYearMonthDateValidation<T>(this IRuleBuilder<T, YearMonth?> ruleBuilder, string errorMessage)
    {
        ruleBuilder.Must(CommonExtensions.BeInThePast).WithMessage(errorMessage);
    }

    public static void FutureYearMonthDateValidation<T>(this IRuleBuilder<T, YearMonth?> ruleBuilder, string errorMessage)
    {
        ruleBuilder.Must(CommonExtensions.BeInTheFuture).WithMessage(errorMessage);
    }

    public static void PastLocalDateValidation<T>(this IRuleBuilder<T, LocalDate?> ruleBuilder, string errorMessage)
    {
        ruleBuilder.Must(CommonExtensions.BeInThePast).WithMessage(errorMessage);
    }

    public static void FutureLocalDateValidation<T>(this IRuleBuilder<T, LocalDate?> ruleBuilder, string errorMessage)
    {
        ruleBuilder.Must(CommonExtensions.BeInTheFuture).WithMessage(errorMessage);
    }

    public static void SocialWorkEnglandNumberValidation<T>(
        this IRuleBuilder<T, string?> ruleBuilder
    )
    {
        ruleBuilder.Custom(
            (sweId, context) =>
            {
                if (string.IsNullOrWhiteSpace(sweId))
                {
                    context.AddFailure(new ValidationFailure("SocialWorkEnglandNumber", "Enter a Social Work England registration number", sweId));
                    return;
                }

                var isSweNumber = SocialWorkEnglandRecord.TryParse(sweId, out _);

                if (!isSweNumber)
                {
                    context.AddFailure(
                        new ValidationFailure(
                            "SocialWorkEnglandNumber",
                            "Enter a Social Work England registration number in the correct format, like SW1234",
                            sweId
                        )
                    );
                }
            }
        );
    }
}
