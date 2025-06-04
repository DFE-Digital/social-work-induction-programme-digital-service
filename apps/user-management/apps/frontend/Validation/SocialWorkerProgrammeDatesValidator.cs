using Dfe.Sww.Ecf.Frontend.Pages.ManageUsers;
using FluentValidation;
using NodaTime;

namespace Dfe.Sww.Ecf.Frontend.Validation;

public class SocialWorkerProgrammeDatesValidator : AbstractValidator<SocialWorkerProgrammeDates>
{
    public SocialWorkerProgrammeDatesValidator()
    {
        When(
            x => x.ProgrammeEndDate.HasValue,
            () => RuleFor(x => x.ProgrammeEndDate)
                .Must(BeInTheFuture)
                .WithMessage("Expected programme end date must be in the future"));
    }

    private bool BeInTheFuture(YearMonth? date)
    {
        var now = DateTime.UtcNow;
        var currentYearMonth = new YearMonth(now.Year, now.Month);

        return date > currentYearMonth;
    }
}
