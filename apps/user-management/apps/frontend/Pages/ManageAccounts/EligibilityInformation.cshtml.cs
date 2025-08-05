using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

public class EligibilityInformation(EcfLinkGenerator linkGenerator) : ManageAccountsBasePageModel
{
    public PageResult OnGet()
    {
        BackLinkPath = linkGenerator.ManageAccount.SelectAccountType(OrganisationId);
        return Page();
    }
}
