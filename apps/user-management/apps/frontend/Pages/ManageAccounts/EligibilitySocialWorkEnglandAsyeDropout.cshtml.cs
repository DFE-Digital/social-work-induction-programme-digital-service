using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

public class EligibilitySocialWorkEnglandAsyeDropout(EcfLinkGenerator linkGenerator) : ManageAccountsBasePageModel
{
    public PageResult OnGet()
    {
        BackLinkPath = FromChangeLink
            ? linkGenerator.ManageAccount.EligibilitySocialWorkEnglandChange(OrganisationId)
            : linkGenerator.ManageAccount.EligibilitySocialWorkEngland(OrganisationId);
        return Page();
    }

    public PageResult OnGetChange()
    {
        FromChangeLink = true;
        return OnGet();
    }
}
