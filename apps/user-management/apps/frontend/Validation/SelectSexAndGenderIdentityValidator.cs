using Dfe.Sww.Ecf.Frontend.Pages.SocialWorkerRegistration;
using FluentValidation;

namespace Dfe.Sww.Ecf.Frontend.Validation;

public class SelectSexAndGenderIdentityValidator : AbstractValidator<SelectSexAndGenderIdentity>
{
    public SelectSexAndGenderIdentityValidator()
    {
        RuleFor(model => model.SelectedUserSex)
            .NotEmpty()
            .WithMessage("Select your sex");

        RuleFor(model => model.GenderMatchesSexAtBirth)
            .NotEmpty()
            .WithMessage("Select your gender identity");
    }
}
