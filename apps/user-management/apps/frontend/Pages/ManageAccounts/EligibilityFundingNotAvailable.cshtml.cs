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
    public string? NextPagePath { get; set; }
    public PageResult OnGet()
    {
        BackLinkPath = createAccountJourneyService.GetIsAgencyWorker() == true
            ? linkGenerator.EligibilityAgencyWorker()
            : linkGenerator.EligibilityQualification();
        var accountDetails = createAccountJourneyService.GetAccountDetails();
        NextPagePath = FromChangeLink && accountDetails?.SocialWorkEnglandNumber is not null ? linkGenerator.ConfirmAccountDetails() : linkGenerator.AddAccountDetails();
        return Page();
    }

    public PageResult OnGetChange()
    {
        FromChangeLink = true;
        return OnGet();
    }
}
