using Dfe.Sww.Ecf.Frontend.Pages.ManageUsers;
using FluentValidation;

namespace Dfe.Sww.Ecf.Frontend.Validation;

public class SelectUserTypeValidator : AbstractValidator<SelectUserType>
{
    public SelectUserTypeValidator()
    {
        RuleFor(model => model.IsStaff)
            .NotEmpty()
            .WithMessage("Select the type of user you want to add");
    }
}
