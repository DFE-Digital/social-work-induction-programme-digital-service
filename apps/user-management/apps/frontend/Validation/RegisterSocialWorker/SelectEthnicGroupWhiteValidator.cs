using Dfe.Sww.Ecf.Frontend.Pages.SocialWorkerRegistration.SelectEthnicGroup;
using FluentValidation;

namespace Dfe.Sww.Ecf.Frontend.Validation.RegisterSocialWorker;

public class SelectEthnicGroupWhiteValidator : AbstractValidator<SelectEthnicGroupWhite>
{
    public SelectEthnicGroupWhiteValidator()
    {
        RuleFor(model => model.SelectedEthnicGroupWhite)
            .NotEmpty()
            .WithMessage("Select an option that best describes your White background");
    }
}
