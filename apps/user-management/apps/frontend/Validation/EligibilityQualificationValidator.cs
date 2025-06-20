using Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;
using FluentValidation;

namespace Dfe.Sww.Ecf.Frontend.Validation;

public class EligibilityQualificationValidator : AbstractValidator<EligibilityQualification>
{
    public EligibilityQualificationValidator()
    {
        RuleFor(model => model.IsRecentlyQualified)
            .NotEmpty()
            .WithMessage("Select if the user completed their social work qualification within the last 3 years");
    }
}
