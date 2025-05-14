using Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;
using FluentValidation;

namespace Dfe.Sww.Ecf.Frontend.Validation;

public class SelectAccountTypeValidator : AbstractValidator<SelectAccountType>
{
    public SelectAccountTypeValidator()
    {
        RuleFor(model => model.IsStaff)
            .NotEmpty()
            .WithMessage("Select the type of user you want to add");
    }
}
