using System.Text.RegularExpressions;
using Dfe.Sww.Ecf.Frontend.Pages.ManageOrganisations;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using FluentValidation;

namespace Dfe.Sww.Ecf.Frontend.Validation.ManageOrganisations;

public partial class EnterLocalAuthorityCodeValidator : AbstractValidator<EnterLocalAuthorityCode>
{
    private readonly IOrganisationService _organisationService;
    public EnterLocalAuthorityCodeValidator(IOrganisationService organisationService)
    {
        _organisationService = organisationService;

        RuleFor(model => model.LocalAuthorityCode)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Enter a local authority code")
            .Custom((laCode, context) =>
            {
                var input = laCode ?? string.Empty;

                if (!DigitsRegex().IsMatch(input))
                {
                    context.AddFailure("The local authority code must only include numbers");
                    return;
                }

                if (!ThreeDigitsRegex().IsMatch(input))
                {
                    context.AddFailure("The local authority code must be three numbers");
                    return;
                }

                if (!LocalAuthorityCodeRegex().IsMatch(input))
                {
                    context.AddFailure("The code you have entered is not associated with a local authority");
                }
            })
            .MustAsync(IsUniqueAsync)
            .WithMessage("An organisation with this local authority code already exists");
    }

    [GeneratedRegex(@"^[1-9]\d{2}$")]
    private static partial Regex LocalAuthorityCodeRegex();

    [GeneratedRegex(@"^\d{3}$")]
    private static partial Regex ThreeDigitsRegex();

    [GeneratedRegex(@"^\d+$")]
    private static partial Regex DigitsRegex();

    private async Task<bool> IsUniqueAsync(
        EnterLocalAuthorityCode model,
        string? laCode,
        ValidationContext<EnterLocalAuthorityCode> context,
        CancellationToken ct)
    {
        if (context.RootContextData.TryGetValue("SkipLaCodeUnique", out var skip) && skip is true)
            return true;

        if (!int.TryParse(laCode, out var code))
            return true; // handled by other validators

        var exists = await _organisationService.ExistsByLocalAuthorityCodeAsync(code);
        return !exists;
    }
}
