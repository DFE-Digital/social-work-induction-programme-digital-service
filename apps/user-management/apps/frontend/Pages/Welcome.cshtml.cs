using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Sww.Ecf.Frontend.Pages;

public class Welcome(EcfLinkGenerator linkGenerator) : BasePageModel
{
    public bool ShowSocialWorkerContent { get; set; } = false;

    public IActionResult OnGet()
    {
        if (User.Identity?.IsAuthenticated != true) return Redirect(linkGenerator.Home());
        if (User.IsInRole(nameof(RoleType.Administrator))) return Redirect(linkGenerator.Dashboard());
        if (User.IsInRole(nameof(RoleType.EarlyCareerSocialWorker))) ShowSocialWorkerContent = true;

        return Page();
    }
}
