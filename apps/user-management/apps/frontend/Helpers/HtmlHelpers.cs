using Microsoft.AspNetCore.Mvc.Rendering;

namespace Dfe.Sww.Ecf.Frontend.Helpers;

public static class HtmlHelpers
{
    private const string DefaultCssClass = "govuk-service-navigation__item--active";

    public static string IsActive(
        this IHtmlHelper html,
        string page,
        bool isAdmin = false
    )
    {
        var currentPage = html.ViewContext.RouteData.Values["page"]?.ToString();

        if (isAdmin)
        {
            return SetAdminActiveClasses(currentPage, page);
        }

        return currentPage?.StartsWith(page, StringComparison.OrdinalIgnoreCase) ?? false
            ? DefaultCssClass
            : string.Empty;
    }

    private static string SetAdminActiveClasses(string? currentPage, string page)
    {
        // Show active class on Manage Organisation nav item when Manage Accounts is accessed by Admin
        if ((currentPage?.Contains("ManageAccounts") ?? false)
            && page.Contains("ManageOrganisations"))
        {
            return DefaultCssClass;
        }

        return currentPage?.StartsWith(page, StringComparison.OrdinalIgnoreCase) ?? false
            ? DefaultCssClass
            : string.Empty;
    }
}
