using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;
using Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;
using Dfe.Sww.Ecf.Frontend.Validation.Common;
using FluentValidation;

namespace Dfe.Sww.Ecf.Frontend.Validation;

public class EligibilitySocialWorkEnglandValidator : AbstractValidator<EligibilitySocialWorkEngland>
{
    public EligibilitySocialWorkEnglandValidator(IAuthServiceClient authServiceClient)
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(model => model.IsRegisteredWithSocialWorkEngland)
            .NotEmpty()
            .WithMessage("Select if the user is registered with Social Work England");

        When(
            x => x.IsRegisteredWithSocialWorkEngland is true,
            () => { RuleFor(y => y.SocialWorkerNumber).SocialWorkEnglandNumberValidation(authServiceClient); }
        );
    }
}
