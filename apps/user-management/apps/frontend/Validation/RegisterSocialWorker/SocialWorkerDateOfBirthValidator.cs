using Dfe.Sww.Ecf.Frontend.Pages.SocialWorkerRegistration;
using Dfe.Sww.Ecf.Frontend.Validation.Common;
using FluentValidation;
using NodaTime;

namespace Dfe.Sww.Ecf.Frontend.Validation.RegisterSocialWorker;

public class SocialWorkerDateOfBirthValidator : AbstractValidator<SelectDateOfBirth>
{
    public SocialWorkerDateOfBirthValidator()
    {
        When(
            x => x.UserDateOfBirth.HasValue,
            () => RuleFor(x => x.UserDateOfBirth)
                .PastLocalDateValidation("Date of birth must be in the past"));
    }
}
