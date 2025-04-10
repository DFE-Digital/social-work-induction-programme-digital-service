using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using SocialWorkInductionProgramme.Frontend.Configuration;
using SocialWorkInductionProgramme.Frontend.Pages.Shared;

namespace SocialWorkInductionProgramme.Frontend.Pages.Debug;

public class DebugBasePageModel(IWebHostEnvironment environment, IOptions<OidcConfiguration> oidcConfiguration)
    : BasePageModel
{
    public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
    {
        if (environment.IsDevelopment() && oidcConfiguration.Value.EnableDevelopmentBackdoor)
        {
            return;
        }

        context.Result = NotFound();
    }
}
