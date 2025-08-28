using Dfe.Sww.Ecf.Frontend.Pages.ManageOrganisations;
using Dfe.Sww.Ecf.Frontend.Validation.Common;
using FluentValidation;

namespace Dfe.Sww.Ecf.Frontend.Validation.ManageOrganisations;

public class EnterPhoneNumberValidator : AbstractValidator<EnterPhoneNumber>
{
    public EnterPhoneNumberValidator()
    {
        RuleFor(model => model.PhoneNumber)
            .Cascade(CascadeMode.Stop)
            .PhoneNumberValidation();
    }
}
