using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageUsers;

/// <summary>
/// Eligibility Information View Model
/// </summary>
[AuthorizeRoles(RoleType.Coordinator)]
public class EligibilityInformation(EcfLinkGenerator linkGenerator) : BasePageModel
{
    public PageResult OnGet()
    {
        BackLinkPath = linkGenerator.SelectUserType();
        return Page();
    }
}
