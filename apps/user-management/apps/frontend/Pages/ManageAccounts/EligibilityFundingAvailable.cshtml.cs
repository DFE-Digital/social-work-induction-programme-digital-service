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
        BackLinkPath = linkGenerator.ManageAccount.EligibilityQualification();
        var accountDetails = createAccountJourneyService.GetAccountDetails();
        if (accountDetails?.SocialWorkEnglandNumber is null)
        {
            NextPagePath = linkGenerator.ManageAccount.AddAccountDetails();
        }
        else if (createAccountJourneyService.GetProgrammeStartDate() is null)
        {
            NextPagePath = linkGenerator.ManageAccount.SocialWorkerProgrammeDates();
        }
        else
        {
            NextPagePath = linkGenerator.ManageAccount.ConfirmAccountDetails();
        }

        return Page();
    }
}
