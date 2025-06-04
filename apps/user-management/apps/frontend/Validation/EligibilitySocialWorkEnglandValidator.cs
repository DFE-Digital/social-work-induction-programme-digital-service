using Dfe.Sww.Ecf.Frontend.Pages.ManageUsers;
using FluentValidation;

namespace Dfe.Sww.Ecf.Frontend.Validation;

public class EligibilitySocialWorkEnglandValidator : AbstractValidator<EligibilitySocialWorkEngland>
{
    public EligibilitySocialWorkEnglandValidator()
    {
        RuleFor(model => model.IsRegisteredWithSocialWorkEngland)
            .NotEmpty()
            .WithMessage("Select if the user is registered with Social Work England");
    }
}
