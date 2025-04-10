using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using SocialWorkInductionProgramme.Frontend.Authorisation;
using SocialWorkInductionProgramme.Frontend.Pages.Shared;
using SocialWorkInductionProgramme.Frontend.Routing;

namespace SocialWorkInductionProgramme.Frontend.Pages;

[AuthorizeRoles(RoleType.Coordinator, RoleType.Assessor, RoleType.EarlyCareerSocialWorker)]
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
