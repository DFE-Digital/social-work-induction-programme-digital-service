using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

/// <summary>
/// Eligibility Social Work England Dropout View Model
/// </summary>
[AuthorizeRoles(RoleType.Coordinator)]
public class EligibilitySocialWorkEnglandDropout(EcfLinkGenerator linkGenerator) : BasePageModel
{
    public PageResult OnGet()
    {
        BackLinkPath = FromChangeLink ? linkGenerator.EligibilitySocialWorkEnglandChange() : linkGenerator.EligibilitySocialWorkEngland();
        return Page();
    }

    public PageResult OnGetChange()
    {
        FromChangeLink = true;
        return OnGet();
    }
}
