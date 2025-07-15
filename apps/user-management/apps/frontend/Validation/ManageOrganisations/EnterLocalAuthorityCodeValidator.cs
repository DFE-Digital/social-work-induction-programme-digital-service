using Dfe.Sww.Ecf.Frontend.Pages.ManageOrganisations;
using FluentValidation;

namespace Dfe.Sww.Ecf.Frontend.Validation.ManageOrganisations;

public class EnterLocalAuthorityCodeValidator : AbstractValidator<EnterLocalAuthorityCode>
{
    public EnterLocalAuthorityCodeValidator()
    {
        // TODO update error message once design is revisited, and unit test for the page
        RuleFor(model => model.LocalAuthorityCode)
            .NotEmpty()
            .WithMessage("Enter the local authority code in full. (error message TBC)");
    }
}
