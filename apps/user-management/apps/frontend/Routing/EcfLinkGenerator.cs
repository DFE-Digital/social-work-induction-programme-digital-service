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

    public string AddAccountDetails() => GetRequiredPathByPage("/ManageUsers/AddAccountDetails");

    public string AddAccountDetailsChange() =>
        GetRequiredPathByPage("/ManageUsers/AddAccountDetails", handler: "Change");

    public string EditAccountDetails(Guid id) =>
        GetRequiredPathByPage("/ManageUsers/EditAccountDetails", routeValues: new { id });

    public string EditAccountDetailsChange(Guid id) =>
        GetRequiredPathByPage(
            "/ManageUsers/EditAccountDetails",
            handler: "Change",
            routeValues: new { id }
        );

    public string ConfirmAccountDetails() =>
        GetRequiredPathByPage("/ManageUsers/ConfirmAccountDetails");

    public string ConfirmAccountDetailsUpdate(Guid id) =>
        GetRequiredPathByPage(
            "/ManageUsers/ConfirmAccountDetails",
            handler: "Update",
            routeValues: new { id }
        );

    public string ViewAccountDetails(Guid id) =>
        GetRequiredPathByPage("/ManageUsers/ViewAccountDetails", routeValues: new { id });

    public string SelectAccountType() => GetRequiredPathByPage("/ManageUsers/SelectAccountType");

    public string AddSomeoneNew() =>
        GetRequiredPathByPage("/ManageUsers/SelectAccountType", handler: "New");

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
