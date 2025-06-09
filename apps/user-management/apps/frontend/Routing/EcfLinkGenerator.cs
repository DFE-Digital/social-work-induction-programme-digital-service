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

    public string ManageAccounts(int? offset = 0, int? pageSize = 10) =>
        GetRequiredPathByPage("/ManageAccounts/Index", routeValues: new { offset, pageSize });

    public string AddAccountDetails() => GetRequiredPathByPage("/ManageAccounts/AddAccountDetails");

    public string AddAccountDetailsChange() =>
        GetRequiredPathByPage("/ManageAccounts/AddAccountDetails", handler: "Change");

    public string EditAccountDetails(Guid id) =>
        GetRequiredPathByPage("/ManageAccounts/EditAccountDetails", routeValues: new { id });

    public string EditAccountDetailsChange(Guid id) =>
        GetRequiredPathByPage(
            "/ManageAccounts/EditAccountDetails",
            handler: "Change",
            routeValues: new { id }
        );

    public string ConfirmAccountDetails() =>
        GetRequiredPathByPage("/ManageAccounts/ConfirmAccountDetails");

    public string ConfirmAccountDetailsUpdate(Guid id) =>
        GetRequiredPathByPage(
            "/ManageAccounts/ConfirmAccountDetails",
            handler: "Update",
            routeValues: new { id }
        );

    public string ViewAccountDetails(Guid id) =>
        GetRequiredPathByPage("/ManageAccounts/ViewAccountDetails", routeValues: new { id });

    public string SelectAccountType() => GetRequiredPathByPage("/ManageAccounts/SelectAccountType");

    public string AddSomeoneNew() =>
        GetRequiredPathByPage("/ManageAccounts/SelectAccountType", handler: "New");

    public string SelectUseCase() => GetRequiredPathByPage("/ManageAccounts/SelectUseCase");

    public string AddExistingUser() => GetRequiredPathByPage("/ManageAccounts/AddExistingUser");

    public string EligibilityInformation() => GetRequiredPathByPage("/ManageAccounts/EligibilityInformation");

    public string EligibilitySocialWorkEngland() => GetRequiredPathByPage("/ManageAccounts/EligibilitySocialWorkEngland");

    public string EligibilitySocialWorkEnglandDropout() => GetRequiredPathByPage("/ManageAccounts/EligibilitySocialWorkEnglandDropout");

    public string EligibilityStatutoryWork() => GetRequiredPathByPage("/ManageAccounts/EligibilityStatutoryWork");

    public string EligibilityStatutoryWorkDropout() => GetRequiredPathByPage("/ManageAccounts/EligibilityStatutoryWorkDropout");

    public string EligibilityAgencyWorker() => GetRequiredPathByPage("/ManageAccounts/EligibilityAgencyWorker");
    public string EligibilityQualification() => GetRequiredPathByPage("/ManageAccounts/EligibilityQualification");
    public string EligibilityFundingNotAvailable() => GetRequiredPathByPage("/ManageAccounts/EligibilityFundingNotAvailable");
    public string EligibilityFundingAvailable() => GetRequiredPathByPage("/ManageAccounts/EligibilityFundingAvailable");
    public string SocialWorkerProgrammeDates() => GetRequiredPathByPage("/ManageAccounts/SocialWorkerProgrammeDates");

    public string SocialWorkerRegistration() => GetRequiredPathByPage("/SocialWorkerRegistration/Index");

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
