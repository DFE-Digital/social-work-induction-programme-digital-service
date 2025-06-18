using Dfe.Sww.Ecf.Frontend.Pages.SocialWorkerRegistration.SelectEthnicGroup;
using FluentValidation;

namespace Dfe.Sww.Ecf.Frontend.Validation.RegisterSocialWorker;

public class SelectEthnicGroupBlackValidator : AbstractValidator<SelectEthnicGroupBlack>
{
    public SelectEthnicGroupBlackValidator()
    {
        RuleFor(model => model.SelectedEthnicGroupBlack)
            .NotEmpty()
            .WithMessage("Select an option that best describes your Black, African, Caribbean or Black British background");
    }
}
