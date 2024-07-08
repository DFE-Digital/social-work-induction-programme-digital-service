using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dfe.Sww.Ecf.UiCommon.FormFlow;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Pages;

[Journey(SignInJourneyState.JourneyName), RequireJourneyInstance]
public class FoundModel(SignInJourneyHelper helper) : PageModel
{
    public JourneyInstance<SignInJourneyState>? JourneyInstance { get; set; }

    public void OnGet()
    {
    }

    public IActionResult OnPost() => helper.GetNextPage(JourneyInstance!).ToActionResult();

    public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
    {
        var state = JourneyInstance!.State;

        if (state.AuthenticationTicket is null)
        {
            // Not matched
            context.Result = helper.GetNextPage(JourneyInstance).ToActionResult();
        }
    }
}
