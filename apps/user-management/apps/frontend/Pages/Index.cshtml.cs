using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Sww.Ecf.Frontend.Pages;

public class Index(EcfLinkGenerator linkGenerator) : BasePageModel
{
    public IActionResult OnGet()
    {
        if (User.Identity?.IsAuthenticated == true) return Redirect(linkGenerator.Welcome());

        return Page();
    }
}
