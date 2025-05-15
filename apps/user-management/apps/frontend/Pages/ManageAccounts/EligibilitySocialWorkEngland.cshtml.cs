using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

/// <summary>
/// Eligibility Social Work England View Model
/// </summary>
[AuthorizeRoles(RoleType.Coordinator)]
public class EligibilitySocialWorkEngland(ICreateAccountJourneyService createAccountJourneyService, EcfLinkGenerator linkGenerator, IValidator<EligibilitySocialWorkEngland> validator)
    : BasePageModel
{
    [BindProperty]
    public bool? IsRegisteredWithSocialWorkEngland { get; set; }

    public PageResult OnGet()
    {
        BackLinkPath = linkGenerator.EligibilityInformation();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var validationResult = await validator.ValidateAsync(this);
        if (IsRegisteredWithSocialWorkEngland is null || !validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            BackLinkPath = linkGenerator.EligibilityInformation();
            return Page();
        }

        createAccountJourneyService.SetIsRegisteredWithSocialWorkEngland(IsRegisteredWithSocialWorkEngland);

        if (IsRegisteredWithSocialWorkEngland is false)
        {
            return Redirect(linkGenerator.AddAccountDetails()); // TODO: Redirect to drop out page in SWIP-590
        }

        return Redirect(linkGenerator.AddAccountDetails()); // TODO: Redirect to statutory work page in SWIP-579
    }
}
