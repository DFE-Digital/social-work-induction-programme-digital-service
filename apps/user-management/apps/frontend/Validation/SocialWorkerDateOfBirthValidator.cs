using Dfe.Sww.Ecf.Frontend.Pages.SocialWorkerRegistration;
using FluentValidation;
using NodaTime;

namespace Dfe.Sww.Ecf.Frontend.Validation;

public class SocialWorkerDateOfBirthValidator : AbstractValidator<SelectDateOfBirth>
{
    public SocialWorkerDateOfBirthValidator()
    {
        When(
            x => x.UserDateOfBirth.HasValue,
            () => RuleFor(x => x.UserDateOfBirth)
                .Must(BeInThePast)
                .WithMessage("Date of birth must be in the past"));
    }

    private bool BeInThePast(LocalDate? date)
    {
        var now = DateTime.UtcNow;
        var currentYearMonth = new LocalDate(now.Year, now.Month, now.Day);

        return date < currentYearMonth;
    }
}
