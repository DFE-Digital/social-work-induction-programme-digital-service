using Dfe.Sww.Ecf.Frontend.Pages.ManageUsers;
using FluentValidation;

namespace Dfe.Sww.Ecf.Frontend.Validation;

public class EligibilityStatutoryWorkValidator : AbstractValidator<EligibilityStatutoryWork>
{
    public EligibilityStatutoryWorkValidator()
    {
        RuleFor(model => model.IsStatutoryWorker)
            .NotEmpty()
            .WithMessage("Select if the user currently works in statutory child and family social work in England");
    }
}
