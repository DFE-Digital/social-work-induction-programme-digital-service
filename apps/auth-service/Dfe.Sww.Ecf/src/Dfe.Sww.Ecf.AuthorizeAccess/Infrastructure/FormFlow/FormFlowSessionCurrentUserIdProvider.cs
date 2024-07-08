using Dfe.Sww.Ecf.SupportUi.Infrastructure.FormFlow;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Infrastructure.FormFlow;

public class FormFlowSessionCurrentUserIdProvider(IHttpContextAccessor httpContextAccessor) : ICurrentUserIdProvider
{
    public string GetCurrentUserId()
    {
        var httpContext = httpContextAccessor.HttpContext ?? throw new InvalidOperationException("No HttpContext.");
        return httpContext.Features.Get<FormFlowSessionIdFeature>()?.SessionId ?? throw new InvalidOperationException($"No {nameof(FormFlowSessionIdFeature)} set.");
    }
}
