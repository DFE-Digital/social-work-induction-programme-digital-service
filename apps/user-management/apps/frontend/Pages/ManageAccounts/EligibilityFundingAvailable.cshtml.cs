using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

/// <summary>
/// Eligibility Funding Available View Model
/// </summary>
[AuthorizeRoles(RoleType.Coordinator)]
public class EligibilityFundingAvailable(
    EcfLinkGenerator linkGenerator,
    ICreateAccountJourneyService createAccountJourneyService
) : BasePageModel
{
    public string? NextPagePath { get; set; }

    public PageResult OnGet()
    {
        BackLinkPath = linkGenerator.EligibilityQualification();
        var accountDetails = createAccountJourneyService.GetAccountDetails();
        if (accountDetails?.SocialWorkEnglandNumber is null)
        {
            NextPagePath = linkGenerator.AddAccountDetails();
        }
        else if (createAccountJourneyService.GetProgrammeStartDate() is null)
        {
            NextPagePath = linkGenerator.SocialWorkerProgrammeDates();
        }
        else
        {
            NextPagePath = linkGenerator.ConfirmAccountDetails();
        }

        return Page();
    }
}
