using Dfe.Sww.Ecf.Frontend.Pages.SocialWorkerRegistration.SelectEthnicGroup;
using FluentValidation;

namespace Dfe.Sww.Ecf.Frontend.Validation.RegisterSocialWorker;

public class SelectEthnicGroupOtherValidator : AbstractValidator<SelectEthnicGroupOther>
{
    public SelectEthnicGroupOtherValidator()
    {
        RuleFor(model => model.SelectedEthnicGroupOther)
            .NotEmpty()
            .WithMessage("Select an option that best describes your background");
    }
}
