using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dfe.Sww.Ecf.UiCommon.FormFlow;
using GovUk.OneLogin.AspNetCore;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Pages;

[Journey(SignInJourneyState.JourneyName), RequireJourneyInstance]
public class SignOutModel : PageModel
{
    public JourneyInstance<SignInJourneyState>? JourneyInstance { get; set; }

    public string ServiceName => JourneyInstance!.State.ServiceName;

    public void OnGet()
    {
    }

    public IActionResult OnPost() =>
        SignOut(
            new AuthenticationProperties()
            {
                RedirectUri = JourneyInstance!.State.ServiceUrl
            },
            OneLoginDefaults.AuthenticationScheme);
}
