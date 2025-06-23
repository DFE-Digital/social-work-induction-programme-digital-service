using Dfe.Sww.Ecf.Frontend.Pages.SocialWorkerRegistration.SelectEthnicGroup;
using FluentValidation;

namespace Dfe.Sww.Ecf.Frontend.Validation.RegisterSocialWorker;

public class SelectEthnicGroupMixedValidator : AbstractValidator<SelectEthnicGroupMixed>
{
    public SelectEthnicGroupMixedValidator()
    {
        RuleFor(model => model.SelectedEthnicGroupMixed)
            .NotEmpty()
            .WithMessage("Select an option that best describes your mixed or multiple ethnic groups background");
    }
}
