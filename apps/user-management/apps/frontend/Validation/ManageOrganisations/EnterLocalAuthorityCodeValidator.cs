using System.Text.RegularExpressions;
using Dfe.Sww.Ecf.Frontend.Pages.ManageOrganisations;
using FluentValidation;

namespace Dfe.Sww.Ecf.Frontend.Validation.ManageOrganisations;

public partial class EnterLocalAuthorityCodeValidator : AbstractValidator<EnterLocalAuthorityCode>
{
    public EnterLocalAuthorityCodeValidator()
    {
        RuleFor(model => model.LocalAuthorityCode)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Enter a local authority code")
            .Must(laCode => laCode != null && MyRegex().IsMatch(laCode.ToString() ?? string.Empty))
            .WithMessage("The local authority code must be three numbers");
    }

    [GeneratedRegex(@"^[1-9]\d{2}$")]
    private static partial Regex MyRegex();
}
