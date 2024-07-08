using Dfe.Analytics;
using Dfe.Analytics.AspNetCore;
using Dfe.Sww.Ecf.AuthorizeAccess.Infrastructure.FormFlow;
using Dfe.Sww.Ecf.UiCommon.FormFlow.State;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Infrastructure.Middleware;

public class AddAnalyticsDataMiddleware(IUserInstanceStateProvider userInstanceStateProvider, RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        var journeyInstance = await userInstanceStateProvider.GetSignInJourneyInstanceAsync(context);
        if (journeyInstance is not null && context.GetWebRequestEvent() is Event webRequestEvent)
        {
            webRequestEvent.Data[DfeAnalyticsEventDataKeys.ApplicationUserId] = [journeyInstance.State.ClientApplicationUserId.ToString()];
        }

        await next(context);
    }
}
