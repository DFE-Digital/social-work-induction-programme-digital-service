using Dfe.Sww.Ecf.Frontend.Configuration;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace Dfe.Sww.Ecf.Frontend.Pages.Debug;

public class DebugBasePageModel(IWebHostEnvironment environment, IOptions<OidcConfiguration> oidcConfiguration)
    : BasePageModel
{
    public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
    {
        if (oidcConfiguration.Value.EnableDevelopmentBackdoor)
        {
            return;
        }

        context.Result = NotFound();
    }
}
