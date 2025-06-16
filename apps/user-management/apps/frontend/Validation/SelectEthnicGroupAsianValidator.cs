using Dfe.Sww.Ecf.Frontend.Pages.SocialWorkerRegistration.SelectEthnicGroup;
using FluentValidation;

namespace Dfe.Sww.Ecf.Frontend.Validation;

public class SelectEthnicGroupAsianValidator : AbstractValidator<SelectEthnicGroupAsian>
{
    public SelectEthnicGroupAsianValidator()
    {
        RuleFor(model => model.SelectedEthnicGroupAsian)
            .NotEmpty()
            .WithMessage("Select an option that best describes your Asian or Asian British background");
    }
}
