using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

/// <summary>
/// Eligibility Funding Not Available View Model
/// </summary>
[AuthorizeRoles(RoleType.Coordinator)]
public class EligibilityFundingNotAvailable(
    ICreateAccountJourneyService createAccountJourneyService,
    EcfLinkGenerator linkGenerator) : BasePageModel
{
    public PageResult OnGet()
    {
        BackLinkPath = createAccountJourneyService.GetIsAgencyWorker() == true
            ? linkGenerator.EligibilityAgencyWorker()
            : linkGenerator.EligibilityQualification();
        return Page();
    }
}
