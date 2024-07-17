using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Validation.Common;
using Dfe.Sww.Ecf.Frontend.Views.Accounts;
using FluentValidation;

namespace Dfe.Sww.Ecf.Frontend.Validation;

/// <summary>
/// Validation for the account model
/// </summary>
public class AddUserDetailsModelValidator : AbstractValidator<AddUserDetailsModel>
{
    public AddUserDetailsModelValidator()
    {
        RuleFor(x => x.FirstName).FirstNameValidation();

        RuleFor(y => y.LastName).LastNameValidation();

        RuleFor(y => y.Email).EmailValidation();
    }
}
