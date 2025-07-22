using Microsoft.AspNetCore.Mvc.Rendering;

namespace Dfe.Sww.Ecf.Frontend.Helpers;

public static class HtmlHelpers
{
    public static string IsActive(
        this IHtmlHelper html,
        string page,
        string cssClass = "govuk-service-navigation__item--active"
    )
    {
        var currentPage = html.ViewContext.RouteData.Values["page"]?.ToString();
        return currentPage?.StartsWith(page, StringComparison.OrdinalIgnoreCase) ?? false
            ? cssClass
            : string.Empty;
    }
}
