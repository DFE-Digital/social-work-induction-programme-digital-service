using Dfe.Sww.Ecf.Frontend.Pages.SocialWorkerRegistration;
using Dfe.Sww.Ecf.Frontend.Validation.Common;
using FluentValidation;
using NodaTime;

namespace Dfe.Sww.Ecf.Frontend.Validation.RegisterSocialWorker;

public class
    SocialWorkerSocialWorkEnglandRegistrationDateValidator : AbstractValidator<SelectSocialWorkEnglandRegistrationDate>
{
    public SocialWorkerSocialWorkEnglandRegistrationDateValidator()
    {
        When(
            x => x.SocialWorkEnglandRegistrationDate.HasValue,
            () => RuleFor(x => x.SocialWorkEnglandRegistrationDate)
                .PastLocalDateValidation("Date were you added to the Social Work England register must be in the past"));
    }
}
