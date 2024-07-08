using Dfe.Sww.Ecf.UiCommon.FormFlow;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Tests;

public static class JourneyInstanceExtensions
{
    public static string GetUniqueIdQueryParameter(this JourneyInstance journeyInstance) =>
        journeyInstance.InstanceId.UniqueKey is string uniqueKey ?
            $"{Constants.UniqueKeyQueryParameterName}={Uri.EscapeDataString(uniqueKey)}" :
            string.Empty;
}
