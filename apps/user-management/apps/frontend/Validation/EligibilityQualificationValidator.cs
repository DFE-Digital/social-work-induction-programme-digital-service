using Dfe.Sww.Ecf.Frontend.Pages.ManageUsers;
using FluentValidation;

namespace Dfe.Sww.Ecf.Frontend.Validation;

public class EligibilityQualificationValidator : AbstractValidator<EligibilityQualification>
{
    public EligibilityQualificationValidator()
    {
        RuleFor(model => model.IsQualifiedWithin3Years)
            .NotEmpty()
            .WithMessage("Select if the user completed their social work qualification within the last 3 years");
    }
}
