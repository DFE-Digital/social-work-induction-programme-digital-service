using Dfe.Sww.Ecf.Frontend.Configuration;
using Dfe.Sww.Ecf.Frontend.Routing;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Dfe.Sww.Ecf.Frontend.Pages.Debug;

public class SignOut(
    IWebHostEnvironment environment,
    IOptions<OidcConfiguration> oidcConfiguration,
    EcfLinkGenerator linkGenerator) : DebugBasePageModel(environment, oidcConfiguration)
{
    public async Task<IActionResult> OnGet()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect(linkGenerator.LoggedOut());
    }
}
