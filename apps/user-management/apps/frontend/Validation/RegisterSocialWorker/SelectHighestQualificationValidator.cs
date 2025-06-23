using Dfe.Sww.Ecf.Frontend.Pages.SocialWorkerRegistration;
using FluentValidation;

namespace Dfe.Sww.Ecf.Frontend.Validation.RegisterSocialWorker;

public class SelectHighestQualificationValidator : AbstractValidator<SelectHighestQualification>
{
    public SelectHighestQualificationValidator()
    {
        RuleFor(model => model.SelectedQualification)
            .NotEmpty()
            .WithMessage("Select your highest qualification");
    }
}
