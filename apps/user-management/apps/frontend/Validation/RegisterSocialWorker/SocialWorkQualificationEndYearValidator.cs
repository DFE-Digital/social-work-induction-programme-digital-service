using Dfe.Sww.Ecf.Frontend.Pages.SocialWorkerRegistration;
using FluentValidation;

namespace Dfe.Sww.Ecf.Frontend.Validation.RegisterSocialWorker;

public class SocialWorkQualificationEndYearValidator : AbstractValidator<SelectSocialWorkQualificationEndYear>
{
    public SocialWorkQualificationEndYearValidator()
    {
        RuleFor(model => model.SocialWorkQualificationEndYear)
            .NotEmpty()
            .WithMessage("Enter the year you finished your social work qualification");

        When(
            x => x.SocialWorkQualificationEndYear.HasValue,
            () => RuleFor(x => x.SocialWorkQualificationEndYear)
                .Must(BeInThePast)
                .WithMessage("The year you finished your social work qualification must be in the past"));

        When(
            x => x.SocialWorkQualificationEndYear.HasValue,
            () => RuleFor(x => x.SocialWorkQualificationEndYear)
                .Must(BeInYearFormat)
                .WithMessage("Enter a real year"));
    }

    private bool BeInThePast(int? year)
    {
        var currentYear = DateTime.UtcNow.Year;

        return year <= currentYear;
    }

    private bool BeInYearFormat(int? year)
    {
        if (year is null)
        {
            return false;
        }

        if (year.Value <= 0)
        {
            return false;
        }

        var yearString = year.ToString();
        if (yearString?.Length != 4)
        {
            return false;
        }

        return true;
    }
}
