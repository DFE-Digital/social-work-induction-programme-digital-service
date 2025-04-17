using Dfe.Sww.Ecf.UiCommon.FormFlow;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Pages;

[Journey(SignInJourneyState.JourneyName), RequireJourneyInstance]
public class NotFoundModel(SignInJourneyHelper helper) : PageModel
{
    public JourneyInstance<SignInJourneyState>? JourneyInstance { get; set; }

    public void OnGet() { }

    public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
    {
        var state = JourneyInstance!.State;

        if (state.AuthenticationTicket is not null)
        {
            // Already matched to an ECF account
            context.Result = Redirect(helper.GetSafeRedirectUri(JourneyInstance));
        }
        else if (state.OneLoginAuthenticationTicket is null || !state.IdentityVerified)
        {
            // Not authenticated/verified with One Login
            context.Result = BadRequest();
        }
    }
}
