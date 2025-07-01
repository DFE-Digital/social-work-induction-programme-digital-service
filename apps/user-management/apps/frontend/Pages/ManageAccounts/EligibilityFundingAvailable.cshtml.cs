using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

/// <summary>
/// Eligibility Funding Available View Model
/// </summary>
[AuthorizeRoles(RoleType.Coordinator)]
public class EligibilityFundingAvailable(EcfLinkGenerator linkGenerator) : BasePageModel
{
    public string? NextPagePath { get; set; }
    public PageResult OnGet()
    {
        BackLinkPath = linkGenerator.EligibilityQualification();
        NextPagePath = FromChangeLink ? linkGenerator.ConfirmAccountDetails() : linkGenerator.AddAccountDetails();
        return Page();
    }

    public PageResult OnGetChange()
    {
        FromChangeLink = true;
        return OnGet();
    }
}
