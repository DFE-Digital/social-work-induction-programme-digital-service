using System.Text.RegularExpressions;

namespace Dfe.Sww.Ecf.Frontend.Routing;

public partial class SlugifyRouteParameterTransformer : IOutboundParameterTransformer
{
    public string? TransformOutbound(object? value)
    {
        return value is not string stringValue
            ? null
            : SlugifyRegex().Replace(stringValue, "$1-$2").ToLower();
    }

    [GeneratedRegex("([a-z])([A-Z])")]
    private static partial Regex SlugifyRegex();
}
