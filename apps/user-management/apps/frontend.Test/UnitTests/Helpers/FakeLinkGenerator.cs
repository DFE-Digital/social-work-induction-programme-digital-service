using Dfe.Sww.Ecf.Frontend.Configuration;
using Dfe.Sww.Ecf.Frontend.Routing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Moq;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;

// TODO: Replace this with an implementation of RoutingEcfLinkGenerator that uses a real LinkGenerator.
//       Will require registering a LinkGenerator in the DI container with all the page routing information.
//       Examples of such can be found on Microsoft's [ASP.NET Core Integration tests](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0) page.
public class FakeLinkGenerator()
    : EcfLinkGenerator(
        new Mock<IWebHostEnvironment>().Object,
        Options.Create(
            new OidcConfiguration()
            {
                AuthorityUrl = "http://localhost",
                CallbackUrl = "http://localhost",
                ClientId = "test",
                ClientSecret = "test",
                CookieName = "test",
                Scopes = [],
                SignedOutCallbackUrl = "http://localhost"
            }
        )
    )
{
    protected override string GetRequiredPathByPage(
        string page,
        string? handler = null,
        object? routeValues = null,
        FragmentString? fragment = null
    )
    {
        const string indexPath = "/index";
        if (page.EndsWith(indexPath, StringComparison.OrdinalIgnoreCase))
        {
            page = page.Remove(page.Length - indexPath.Length);
        }

        var generatedLink = new SlugifyRouteParameterTransformer().TransformOutbound(page)!;

        // This is needed the home page redirects where the string is empty, which can't be passed into the redirect method
        if (string.IsNullOrEmpty(generatedLink))
        {
            return "index";
        }

        if (routeValues is not null)
            generatedLink = AddRouteValues(generatedLink, routeValues);

        if  (fragment is not null)
            generatedLink = AddFragment(generatedLink, (FragmentString)fragment);

        return handler is null
            ? generatedLink
            : QueryHelpers.AddQueryString(generatedLink, "handler", handler);
    }

    protected override string GetRequiredUriByPage(
        HttpContext httpContext,
        string page,
        string? handler = null,
        object? routeValues = null
    )
    {
        const string indexPath = "/index";
        if (page.EndsWith(indexPath, StringComparison.OrdinalIgnoreCase))
        {
            page = page.Remove(page.Length - indexPath.Length);
        }

        var generatedLink = new SlugifyRouteParameterTransformer().TransformOutbound(page)!;

        if (routeValues is not null)
            generatedLink = AddRouteValues(generatedLink, routeValues);

        generatedLink = handler is null
            ? generatedLink
            : QueryHelpers.AddQueryString(generatedLink, "handler", handler);

        return httpContext.Request.Scheme + "://" + httpContext.Request.Host + generatedLink;
    }

    private static string AddFragment(string link, FragmentString fragment)
    {
        return link + fragment;
    }

    private static string AddRouteValues(string link, object routeValues)
    {
        return routeValues
            .GetType()
            .GetProperties()
            .Aggregate(
                link,
                (current, prop) =>
                    current
                    + prop.Name switch
                    {
                        "id" => prop.GetValue(routeValues) != null ? "/" + prop.GetValue(routeValues) : string.Empty,
                        "linkingToken"
                            => QueryHelpers.AddQueryString(
                                link,
                                "linkingToken",
                                prop.GetValue(routeValues)?.ToString() ?? string.Empty
                            ),
                        _ => string.Empty
                    }
            );
    }
}
