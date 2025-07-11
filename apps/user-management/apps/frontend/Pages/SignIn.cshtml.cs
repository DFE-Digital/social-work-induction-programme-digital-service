using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Sww.Ecf.Frontend.Pages;

[Authorize]
public class SignIn(EcfLinkGenerator linkGenerator, IAuthServiceClient authServiceClient) : BasePageModel
{
    public IActionResult OnGet()
    {
        var isEcswRegistered = authServiceClient.HttpContextService.GetIsEcswRegistered();
        if (isEcswRegistered == false)
        {
            return Redirect(linkGenerator.SocialWorkerRegistration());
        }

        if (HttpContext.User.Identity?.IsAuthenticated == true && User.IsInRole(RoleType.Administrator.ToString()))
        {
            return Redirect(linkGenerator.Dashboard());
        }

        return Redirect(linkGenerator.Home());
    }
}
