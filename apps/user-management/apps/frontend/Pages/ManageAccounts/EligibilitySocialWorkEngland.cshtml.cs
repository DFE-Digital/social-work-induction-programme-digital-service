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
public class EligibilitySocialWorkEngland(
    ICreateAccountJourneyService createAccountJourneyService,
    EcfLinkGenerator linkGenerator,
    IValidator<EligibilitySocialWorkEngland> validator)
    : BasePageModel
{
    [BindProperty] public bool? IsRegisteredWithSocialWorkEngland { get; set; }

    public PageResult OnGet()
    {
        BackLinkPath = FromChangeLink ? linkGenerator.ConfirmAccountDetails() : linkGenerator.EligibilityInformation();
        IsRegisteredWithSocialWorkEngland = createAccountJourneyService.GetIsRegisteredWithSocialWorkEngland();
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
        if (FromChangeLink)
        {
            if (IsRegisteredWithSocialWorkEngland == true)
            {
                return Redirect(linkGenerator.ConfirmAccountDetails());
            }
            return Redirect(linkGenerator.EligibilitySocialWorkEnglandDropoutChange());
        }
        return Redirect(IsRegisteredWithSocialWorkEngland is false
            ? linkGenerator.EligibilitySocialWorkEnglandDropout()
            : linkGenerator.EligibilityStatutoryWork());
    }

    public PageResult OnGetChange()
    {
        FromChangeLink = true;
        return OnGet();
    }

    public async Task<IActionResult> OnPostChangeAsync()
    {
        FromChangeLink = true;
        return await OnPostAsync();
    }
}
