using Dfe.Sww.Ecf.Frontend.Authorisation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Dfe.Sww.Ecf.Frontend.Helpers;

public static class HtmlHelpers
{
    public static string IsActive(
        this IHtmlHelper html,
        string page,
        string cssClass = "govuk-service-navigation__item--active",
        RoleType? activeWhenRoleIs = null,
        string? activePageOverride = null)
    {
        var routePage = html.ViewContext.RouteData.Values["page"]?.ToString();
        var user = html.ViewContext.HttpContext.User;

        if (string.IsNullOrEmpty(routePage))
            return string.Empty;

        if (IsRoleBasedOverrideActive(
                user,
                routePage,
                activeWhenRoleIs,
                activePageOverride))
            return cssClass;

        return routePage.StartsWith(page, StringComparison.OrdinalIgnoreCase)
            ? cssClass
            : string.Empty;
    }

    private static bool IsRoleBasedOverrideActive(
        System.Security.Claims.ClaimsPrincipal user,
        string routePage,
        RoleType? role,
        string? overridePage)
    {
        if (role is null || string.IsNullOrEmpty(overridePage))
            return false;

        var userHasRole = user.IsInRole(role.Value.ToString());
        var isOnOverridePage = routePage.StartsWith(
            overridePage,
            StringComparison.OrdinalIgnoreCase);

        return userHasRole && isOnOverridePage;
    }
}
