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
        var hasAccountDetails = HasAccountDetails();

        if (!hasAccountDetails)
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

    private bool HasAccountDetails()
    {
        var isEcsw = createAccountJourneyService.GetAccountTypes()?.Contains(AccountType.EarlyCareerSocialWorker) ?? false;
        var accountDetails = createAccountJourneyService.GetAccountDetails();

        if (accountDetails is null)
        {
            return false;
        }

        // ECSWs have a SWE ID but no account details at this stage
        if (isEcsw && string.IsNullOrWhiteSpace(accountDetails.Email))
        {
            return false;
        }

        // Other types don't have SWE ID or account details
        if (accountDetails.SocialWorkEnglandNumber is null)
        {
            return false;
        }

        return true;
    }
}
