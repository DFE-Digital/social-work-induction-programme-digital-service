using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Validation.Common;
using FluentValidation;

namespace Dfe.Sww.Ecf.Frontend.Validation;

/// <summary>
/// Validation for the user model
/// </summary>
public class UserDetailsValidator : AbstractValidator<UserDetails>
{
    public UserDetailsValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(y => y.FirstName).FirstNameValidation();
        RuleFor(y => y.LastName).LastNameValidation();
        RuleFor(y => y.Email).EmailValidation();
        When(
            x => x.IsStaff == false,
            () => { RuleFor(y => y.SocialWorkEnglandNumber).SocialWorkEnglandNumberValidation(); }
        );
    }
}
