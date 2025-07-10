using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Sww.Ecf.Frontend.Pages;

[AuthorizeRoles(RoleType.Coordinator, RoleType.Assessor, RoleType.EarlyCareerSocialWorker, RoleType.Administrator)]
public class SignOut(EcfLinkGenerator linkGenerator) : BasePageModel
{
    public async Task<IActionResult> OnGetAsync()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignOutAsync(
            OpenIdConnectDefaults.AuthenticationScheme,
            new AuthenticationProperties { RedirectUri = linkGenerator.LoggedOut() }
        );
        return Page();
    }
}
