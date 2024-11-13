using Dfe.Sww.Ecf.UiCommon.FormFlow;
using Microsoft.AspNetCore.WebUtilities;

namespace Dfe.Sww.Ecf.AuthorizeAccess;

public abstract class AuthorizeAccessLinkGenerator
{
    public string DebugIdentity(JourneyInstanceId journeyInstanceId) =>
        GetRequiredPathByPage("/DebugIdentity", journeyInstanceId: journeyInstanceId);

    public string NotVerified(JourneyInstanceId journeyInstanceId) =>
        GetRequiredPathByPage("/NotVerified", journeyInstanceId: journeyInstanceId);

    public string NotFound(JourneyInstanceId journeyInstanceId) =>
        GetRequiredPathByPage("/NotFound", journeyInstanceId: journeyInstanceId);

    public string SignOut(JourneyInstanceId journeyInstanceId) =>
        GetRequiredPathByPage("/SignOut", journeyInstanceId: journeyInstanceId);

    protected virtual string GetRequiredPathByPage(
        string page,
        string? handler = null,
        object? routeValues = null,
        JourneyInstanceId? journeyInstanceId = null
    )
    {
        var url = GetRequiredPathByPage(page, handler, routeValues);

        if (journeyInstanceId?.UniqueKey is { } journeyInstanceUniqueKey)
        {
            url = QueryHelpers.AddQueryString(
                url,
                Constants.UniqueKeyQueryParameterName,
                journeyInstanceUniqueKey
            );
        }

        return url;
    }

    protected abstract string GetRequiredPathByPage(
        string page,
        string? handler = null,
        object? routeValues = null
    );
}

public class RoutingAuthorizeAccessLinkGenerator(LinkGenerator linkGenerator)
    : AuthorizeAccessLinkGenerator
{
    protected override string GetRequiredPathByPage(
        string page,
        string? handler = null,
        object? routeValues = null
    ) =>
        linkGenerator.GetPathByPage(page, handler, values: routeValues)
        ?? throw new InvalidOperationException("Page was not found.");
}
