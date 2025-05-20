using Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;
using FluentValidation;

namespace Dfe.Sww.Ecf.Frontend.Validation;

public class EligibilityAgencyWorkerValidator : AbstractValidator<EligibilityAgencyWorker>
{
    public EligibilityAgencyWorkerValidator()
    {
        RuleFor(model => model.IsAgencyWorker)
            .NotEmpty()
            .WithMessage("Select if the user is an agency worker");
    }
}
