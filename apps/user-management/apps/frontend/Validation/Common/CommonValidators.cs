using System.Text.RegularExpressions;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;
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

    public static void EmailValidation<T>(this IRuleBuilder<T, string?> ruleBuilder, Func<string, CancellationToken, Task<bool>> emailExistsAsync)
    {
        ruleBuilder
            .NotEmpty()
            .WithMessage("Enter an email address")
            .EmailAddress()
            .WithMessage("Enter an email address in the correct format, like name@example.com")
            .MustAsync(
                async (_, email, context, ct) =>
                {
                    if (context.RootContextData.TryGetValue("SkipEmailUnique", out var skip) && skip is true)
                        return true;

                    var exists = await emailExistsAsync(email, ct);
                    return !exists;
                }
            )
            .WithMessage("The email address entered belongs to an existing user");
    }

    public static void PhoneNumberValidation<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        const string errorMessage = "Enter a phone number, like 01632 960 001, 07700 900 982 or +44 808 157 0192";

        ruleBuilder
            .NotEmpty()
            .WithMessage(errorMessage)
            .Must(phoneNumber =>
            {
                if (string.IsNullOrWhiteSpace(phoneNumber))
                    return false;

                var phoneNumberNoSpaces = Regex.Replace(phoneNumber, @"\s+", "");
                return Regex.IsMatch(phoneNumberNoSpaces, @"^(?:\+447\d{9}|07\d{9}|0[12]\d{8,9}|\+44[12]\d{8,9})$");
            })
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
        this IRuleBuilder<T, string?> ruleBuilder,
        IAuthServiceClient authServiceClient
    )
    {
        ruleBuilder.CustomAsync(async (sweId, context, ct) =>
            {
                if (string.IsNullOrWhiteSpace(sweId))
                {
                    context.AddFailure(new ValidationFailure(
                        context.PropertyPath,
                        "Enter a Social Work England registration number",
                        sweId));
                    return;
                }

                var isValid = context.RootContextData.TryGetValue("SkipSweIdUnique", out var isSkipped);
                if (isValid && isSkipped is false)
                {
                    var account = await authServiceClient.Accounts.GetBySocialWorkEnglandNumberAsync(sweId);
                    if (account is not null)
                    {
                        context.AddFailure(new ValidationFailure(
                            context.PropertyPath,
                            "The Social Work England registration number entered belongs to an existing user",
                            sweId));
                    }
                }

                var isSweNumber = SocialWorkEnglandRecord.TryParse(sweId, out _);

                if (!isSweNumber)
                {
                    context.AddFailure(
                        new ValidationFailure(
                            context.PropertyPath,
                            "Enter a Social Work England registration number in the correct format",
                            sweId
                        )
                    );
                }
            }
        );
    }
}
