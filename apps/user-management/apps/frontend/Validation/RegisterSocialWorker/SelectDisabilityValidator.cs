using Dfe.Sww.Ecf.Frontend.Pages.SocialWorkerRegistration;
using FluentValidation;

namespace Dfe.Sww.Ecf.Frontend.Validation.RegisterSocialWorker;

public class SelectDisabilityValidator : AbstractValidator<SelectDisability>
{
    public SelectDisabilityValidator()
    {
        RuleFor(model => model.IsDisabled)
            .NotEmpty()
            .WithMessage("Select yes if you consider yourself to have a disability");
    }
}
