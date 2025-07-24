using Dfe.Sww.Ecf.Frontend.Pages.ManageOrganisations;
using FluentValidation;

namespace Dfe.Sww.Ecf.Frontend.Validation.ManageOrganisations;

public class EditPrimaryCoordinatorChangeTypeValidator : AbstractValidator<EditPrimaryCoordinatorChangeType>
{
    public EditPrimaryCoordinatorChangeTypeValidator()
    {
        RuleFor(model => model.ChangeType)
            .NotEmpty()
            .WithMessage("Select the type of change you are making to the primary coordinator");
    }
}
