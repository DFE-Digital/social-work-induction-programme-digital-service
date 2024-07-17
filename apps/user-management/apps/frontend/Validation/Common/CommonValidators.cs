using FluentValidation;

namespace Dfe.Sww.Ecf.Frontend.Validation.Common;

public static class CommonValidators
{
    // TO DO - Update these in validation task
    public static void FirstNameValidation<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        ruleBuilder.NotEmpty().WithMessage("Oops First Name is empty");
    }

    public static void LastNameValidation<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        ruleBuilder.NotEmpty().WithMessage("Oops Last Name is empty");
    }

    public static void EmailValidation<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        ruleBuilder
            .NotEmpty().WithMessage("Oops Email is empty")
            .EmailAddress().WithMessage("Oops that isn't an email");
    }
}
