using Dfe.Sww.Ecf.Frontend.Pages.SocialWorkerRegistration;
using FluentValidation;

namespace Dfe.Sww.Ecf.Frontend.Validation;

public class SelectEthnicGroupValidator : AbstractValidator<SelectEthnicGroup>
{
    public SelectEthnicGroupValidator()
    {
        RuleFor(model => model.SelectedEthnicGroup)
            .NotEmpty()
            .WithMessage("Select your ethnic group");
    }
}
