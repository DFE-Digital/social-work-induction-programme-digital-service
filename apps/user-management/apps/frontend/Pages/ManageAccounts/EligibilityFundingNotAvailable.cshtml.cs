using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

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
