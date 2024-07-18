using FluentValidation;

namespace Dfe.Sww.Ecf.Frontend.Validation.Common;

public static class CommonValidators
{
    public static void FirstNameValidation<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        ruleBuilder.NotEmpty().WithMessage("Enter a first or last name");
    }

    public static void LastNameValidation<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        ruleBuilder.NotEmpty().WithMessage("Enter a first or last name");
    }

    public static void EmailValidation<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        ruleBuilder
            .NotEmpty().WithMessage("Enter an email")
            .EmailAddress().WithMessage("Enter an email address in the correct format, like name@example.com");
    }
}
