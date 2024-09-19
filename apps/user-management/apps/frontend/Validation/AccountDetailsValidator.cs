using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Validation.Common;
using FluentValidation;

namespace Dfe.Sww.Ecf.Frontend.Validation;

/// <summary>
/// Validation for the account model
/// </summary>
public class AccountDetailsValidator : AbstractValidator<AccountDetails>
{
    public AccountDetailsValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        When(
            x => string.IsNullOrWhiteSpace(x.LastName),
            () =>
            {
                RuleFor(y => y.FirstName).FirstNameValidation();
            }
        );

        When(
            x => string.IsNullOrWhiteSpace(x.FirstName),
            () =>
            {
                RuleFor(y => y.LastName).LastNameValidation();
            }
        );

        RuleFor(y => y.Email).EmailValidation();

        RuleFor(x => x.SocialWorkEnglandNumber).SocialWorkEnglandNumberValidation();
    }
}
