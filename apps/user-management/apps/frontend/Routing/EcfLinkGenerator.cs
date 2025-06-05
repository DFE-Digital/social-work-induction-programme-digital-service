using Dfe.Sww.Ecf.Frontend.Configuration;
using Microsoft.Extensions.Options;

namespace Dfe.Sww.Ecf.Frontend.Routing;

public abstract class EcfLinkGenerator(
    IWebHostEnvironment environment,
    IOptions<OidcConfiguration> oidcConfiguration
)
{
    private bool IsBackdoorEnabled => oidcConfiguration.Value.EnableDevelopmentBackdoor;

    public string SignIn() =>
        IsBackdoorEnabled
            ? GetRequiredPathByPage("/Debug/Backdoor")
            : GetRequiredPathByPage("/SignIn");

    public string SignInWithLinkingToken(HttpContext httpContext, string linkingToken) =>
        GetRequiredUriByPage(
            httpContext,
            "/SignIn",
            handler: "invite",
            routeValues: new { linkingToken }
        );

    public string SignOut() =>
        IsBackdoorEnabled
            ? GetRequiredPathByPage("/Debug/SignOut")
            : GetRequiredPathByPage("/SignOut");

    public string LoggedOut() => GetRequiredPathByPage("/LoggedOut");

    public string Home() => GetRequiredPathByPage("/Index");

    public string ManageUsers(int? offset = 0, int? pageSize = 10) =>
        GetRequiredPathByPage("/ManageUsers/Index", routeValues: new { offset, pageSize });

    public string AddUserDetails() => GetRequiredPathByPage("/ManageUsers/AddUserDetails");

    public string AddUserDetailsChange() =>
        GetRequiredPathByPage("/ManageUsers/AddUserDetails", handler: "Change");

    public string EditUserDetails(Guid id) =>
        GetRequiredPathByPage("/ManageUsers/EditUserDetails", routeValues: new { id });

    public string EditUserDetailsChange(Guid id) =>
        GetRequiredPathByPage(
            "/ManageUsers/EditUserDetails",
            handler: "Change",
            routeValues: new { id }
        );

    public string ConfirmUserDetails() =>
        GetRequiredPathByPage("/ManageUsers/ConfirmUserDetails");

    public string ConfirmUserDetailsUpdate(Guid id) =>
        GetRequiredPathByPage(
            "/ManageUsers/ConfirmUserDetails",
            handler: "Update",
            routeValues: new { id }
        );

    public string ViewUserDetails(Guid id) =>
        GetRequiredPathByPage("/ManageUsers/ViewUserDetails", routeValues: new { id });

    public string SelectUserType() => GetRequiredPathByPage("/ManageUsers/SelectUserType");

    public string AddSomeoneNew() =>
        GetRequiredPathByPage("/ManageUsers/SelectUserType", handler: "New");

    public string SelectUseCase() => GetRequiredPathByPage("/ManageUsers/SelectUseCase");

    public string AddExistingUser() => GetRequiredPathByPage("/ManageUsers/AddExistingUser");

    public string EligibilityInformation() => GetRequiredPathByPage("/ManageUsers/EligibilityInformation");

    public string EligibilitySocialWorkEngland() => GetRequiredPathByPage("/ManageUsers/EligibilitySocialWorkEngland");

    public string EligibilitySocialWorkEnglandDropout() => GetRequiredPathByPage("/ManageUsers/EligibilitySocialWorkEnglandDropout");

    public string EligibilityStatutoryWork() => GetRequiredPathByPage("/ManageUsers/EligibilityStatutoryWork");

    public string EligibilityStatutoryWorkDropout() => GetRequiredPathByPage("/ManageUsers/EligibilityStatutoryWorkDropout");

    public string EligibilityAgencyWorker() => GetRequiredPathByPage("/ManageUsers/EligibilityAgencyWorker");
    public string EligibilityQualification() => GetRequiredPathByPage("/ManageUsers/EligibilityQualification");
    public string EligibilityFundingNotAvailable() => GetRequiredPathByPage("/ManageUsers/EligibilityFundingNotAvailable");
    public string EligibilityFundingAvailable() => GetRequiredPathByPage("/ManageUsers/EligibilityFundingAvailable");
    public string SocialWorkerProgrammeDates() => GetRequiredPathByPage("/ManageUsers/SocialWorkerProgrammeDates");

    protected abstract string GetRequiredPathByPage(
        string page,
        string? handler = null,
        object? routeValues = null
    );

    protected abstract string GetRequiredUriByPage(
        HttpContext httpContext,
        string page,
        string? handler = null,
        object? routeValues = null
    );
}

public class RoutingEcfLinkGenerator(
    IWebHostEnvironment environment,
    IOptions<OidcConfiguration> oidcConfiguration,
    LinkGenerator linkGenerator
) : EcfLinkGenerator(environment, oidcConfiguration)
{
    protected override string GetRequiredPathByPage(
        string page,
        string? handler = null,
        object? routeValues = null
    )
    {
        return linkGenerator.GetPathByPage(page, handler, values: routeValues)
            ?? throw new InvalidOperationException("Page was not found.");
    }

    protected override string GetRequiredUriByPage(
        HttpContext httpContext,
        string page,
        string? handler = null,
        object? routeValues = null
    )
    {
        return linkGenerator.GetUriByPage(httpContext, page, handler, values: routeValues)
            ?? throw new InvalidOperationException("Page was not found.");
    }
}
