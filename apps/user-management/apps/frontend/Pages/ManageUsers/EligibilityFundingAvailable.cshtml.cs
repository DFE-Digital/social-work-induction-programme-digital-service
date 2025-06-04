using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageUsers;

/// <summary>
/// Eligibility Funding Available View Model
/// </summary>
[AuthorizeRoles(RoleType.Coordinator)]
public class EligibilityFundingAvailable(EcfLinkGenerator linkGenerator) : BasePageModel
{
    public PageResult OnGet()
    {
        BackLinkPath = linkGenerator.EligibilityQualification();
        return Page();
    }
}
