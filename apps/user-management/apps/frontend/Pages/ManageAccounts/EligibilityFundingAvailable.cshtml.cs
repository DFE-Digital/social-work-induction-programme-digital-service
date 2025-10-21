using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

public class EligibilityFundingAvailable(
    EcfLinkGenerator linkGenerator,
    ICreateAccountJourneyService createAccountJourneyService
) : ManageAccountsBasePageModel
{
    public string? NextPagePath { get; set; }

    public PageResult OnGet()
    {
        BackLinkPath = linkGenerator.ManageAccount.EligibilityQualification(OrganisationId);
        var isEcsw = createAccountJourneyService.GetAccountTypes()?.Contains(AccountType.EarlyCareerSocialWorker) ?? false;
        var accountDetails = createAccountJourneyService.GetAccountDetails();

        // ECSWs have a SWE ID but no account details at this stage, other account types don't have an ID or account details
        if ((isEcsw && accountDetails?.Email is null)
            || accountDetails?.SocialWorkEnglandNumber is null)
        {
            NextPagePath = linkGenerator.ManageAccount.AddAccountDetails(OrganisationId);
        }
        else if (createAccountJourneyService.GetProgrammeStartDate() is null)
        {
            NextPagePath = linkGenerator.ManageAccount.SocialWorkerProgrammeDates(OrganisationId);
        }
        else
        {
            NextPagePath = linkGenerator.ManageAccount.ConfirmAccountDetails(OrganisationId);
        }

        return Page();
    }
}
