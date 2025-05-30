using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dfe.Sww.Ecf.UiCommon.FormFlow;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Pages;

[Journey(SignInJourneyState.JourneyName), RequireJourneyInstance]
public class NotVerifiedModel(SignInJourneyHelper helper) : PageModel
{
    public JourneyInstance<SignInJourneyState>? JourneyInstance { get; set; }

    public void OnGet()
    {
    }

    public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
    {
        var state = JourneyInstance!.State;

        if (state.AuthenticationTicket is not null)
        {
            // Already matched to a Teaching Record
            context.Result = Redirect(helper.GetSafeRedirectUri(JourneyInstance));
        }
        else if (state.OneLoginAuthenticationTicket is null)
        {
            // Not authenticated with One Login
            context.Result = BadRequest();
        }
        else if (state.IdentityVerified)
        {
            // Verified specified NINO or TRN
            context.Result = BadRequest();
        }
    }
}
