using FluentValidation;
using Index = Dfe.Sww.Ecf.Frontend.Pages.SocialWorkerRegistration.SelectEthnicGroup.Index;

namespace Dfe.Sww.Ecf.Frontend.Validation.RegisterSocialWorker;

public class SelectEthnicGroupValidator : AbstractValidator<Index>
{
    public SelectEthnicGroupValidator()
    {
        RuleFor(model => model.SelectedEthnicGroup)
            .NotEmpty()
            .WithMessage("Select your ethnic group");
    }
}
