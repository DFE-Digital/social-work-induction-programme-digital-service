using Dfe.Sww.Ecf.Frontend.Models;
using FluentValidation;

namespace Dfe.Sww.Ecf.Frontend.Validation;

/// <summary>
/// Validation for the account model
/// </summary>
public class AccountValidator : AbstractValidator<Account>
{
    public AccountValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty();

        RuleFor(y => y.LastName).NotEmpty();

        RuleFor(y => y.Email).NotEmpty();
    }
}
