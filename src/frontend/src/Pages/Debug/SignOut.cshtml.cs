using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SocialWorkInductionProgramme.Frontend.Configuration;
using SocialWorkInductionProgramme.Frontend.Routing;

namespace SocialWorkInductionProgramme.Frontend.Pages.Debug;

public class SignOut(
    IWebHostEnvironment environment,
    IOptions<OidcConfiguration> oidcConfiguration,
    EcfLinkGenerator linkGenerator
) : DebugBasePageModel(environment, oidcConfiguration)
{
    public async Task<IActionResult> OnGetAsync()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect(linkGenerator.LoggedOut());
    }
}
