using Dfe.Sww.Ecf.Frontend.Pages.SocialWorkerRegistration.SelectEthnicGroup;
using FluentValidation;

namespace Dfe.Sww.Ecf.Frontend.Validation;

public class SelectWhiteEthnicGroupValidator : AbstractValidator<SelectWhiteEthnicGroup>
{
    public SelectWhiteEthnicGroupValidator()
    {
        RuleFor(model => model.SelectedEthnicGroupWhite)
            .NotEmpty()
            .WithMessage("Select an option that best describes your White background");
    }
}
