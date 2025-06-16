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
/// Eligibility Qualification View Model
/// </summary>
[AuthorizeRoles(RoleType.Coordinator)]
public class EligibilityQualification(
    ICreateAccountJourneyService createAccountJourneyService,
    EcfLinkGenerator linkGenerator,
    IValidator<EligibilityQualification> validator)
    : BasePageModel
{
    /// <summary>
    /// Property capturing whether the user has recently completed their social work qualification.
    /// </summary>
    /// <returns>True if the user has qualified in the last 3 years, false otherwise.</returns>
    [BindProperty] public bool? IsRecentlyQualified { get; set; }

    public PageResult OnGet()
    {
        BackLinkPath = linkGenerator.EligibilityAgencyWorker();
        IsRecentlyQualified = createAccountJourneyService.GetIsRecentlyQualified();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var validationResult = await validator.ValidateAsync(this);
        if (IsRecentlyQualified is null || !validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            BackLinkPath = linkGenerator.EligibilityAgencyWorker();
            return Page();
        }

        createAccountJourneyService.SetIsRecentlyQualified(IsRecentlyQualified);

        return Redirect(IsRecentlyQualified is false
            ? linkGenerator.EligibilityFundingNotAvailable()
            : linkGenerator.EligibilityFundingAvailable());
    }
}
