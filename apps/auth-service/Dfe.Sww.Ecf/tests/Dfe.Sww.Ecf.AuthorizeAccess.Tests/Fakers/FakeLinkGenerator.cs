namespace Dfe.Sww.Ecf.AuthorizeAccess.Tests.Fakers;

public class FakeLinkGenerator : AuthorizeAccessLinkGenerator
{
    protected override string GetRequiredPathByPage(
        string page,
        string? handler = null,
        object? routeValues = null
    )
    {
        return page;
    }
}
