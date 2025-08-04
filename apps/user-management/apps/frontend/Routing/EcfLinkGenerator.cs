using Dfe.Sww.Ecf.Frontend.Configuration;
using Microsoft.Extensions.Options;

namespace Dfe.Sww.Ecf.Frontend.Routing;

public abstract class EcfLinkGenerator
{
    private readonly OidcConfiguration _oidcConfiguration;

    protected EcfLinkGenerator(
        IOptions<OidcConfiguration> oidcConfiguration
    )
    {
        _oidcConfiguration = oidcConfiguration.Value;
        ManageOrganisations = new(this);
        ManageAccount = new(this);
    }

    public ManageOrganisationLinks ManageOrganisations { get; set; }
    public ManageAccountLinks ManageAccount { get; set; }

    private bool IsBackdoorEnabled => _oidcConfiguration.EnableDevelopmentBackdoor;

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

    public string Welcome() => GetRequiredPathByPage("/Welcome");

    public string Dashboard() => GetRequiredPathByPage("/Dashboard");

    // SWE registration links
    public string SocialWorkerRegistration() => GetRequiredPathByPage("/SocialWorkerRegistration/Index");
    public string SocialWorkerRegistrationDateOfBirth() => GetRequiredPathByPage("/SocialWorkerRegistration/SelectDateOfBirth");
    public string SocialWorkerRegistrationSexAndGenderIdentity() => GetRequiredPathByPage("/SocialWorkerRegistration/SelectSexAndGenderIdentity");
    public string SocialWorkerRegistrationEthnicGroup() => GetRequiredPathByPage("/SocialWorkerRegistration/SelectEthnicGroup/Index");
    public string SocialWorkerRegistrationEthnicGroupWhite() => GetRequiredPathByPage("/SocialWorkerRegistration/SelectEthnicGroup/White");
    public string SocialWorkerRegistrationEthnicGroupMixed() => GetRequiredPathByPage("/SocialWorkerRegistration/SelectEthnicGroup/MixedOrMultipleEthnicGroups");
    public string SocialWorkerRegistrationEthnicGroupAsian() => GetRequiredPathByPage("/SocialWorkerRegistration/SelectEthnicGroup/AsianOrAsianBritish");
    public string SocialWorkerRegistrationEthnicGroupBlack() => GetRequiredPathByPage("/SocialWorkerRegistration/SelectEthnicGroup/BlackAfricanCaribbeanOrBlackBritish");
    public string SocialWorkerRegistrationEthnicGroupOther() => GetRequiredPathByPage("/SocialWorkerRegistration/SelectEthnicGroup/OtherEthnicGroup");
    public string SocialWorkerRegistrationSelectDisability() => GetRequiredPathByPage("/SocialWorkerRegistration/SelectDisability");

    public string SocialWorkerRegistrationSelectSocialWorkEnglandRegistrationDate() =>
        GetRequiredPathByPage("/SocialWorkerRegistration/SelectSocialWorkEnglandRegistrationDate");

    public string SocialWorkerRegistrationSelectHighestQualification() => GetRequiredPathByPage("/SocialWorkerRegistration/SelectHighestQualification");
    public string SocialWorkerRegistrationSelectSocialWorkQualificationEndYear() => GetRequiredPathByPage("/SocialWorkerRegistration/SelectSocialWorkQualificationEndYear");
    public string SocialWorkerRegistrationSelectRouteIntoSocialWork() => GetRequiredPathByPage("/SocialWorkerRegistration/SelectRouteIntoSocialWork");
    public string SocialWorkerRegistrationCheckYourAnswers() => GetRequiredPathByPage("/SocialWorkerRegistration/CheckYourAnswers");
    public string SocialWorkerRegistrationRegistrationComplete() => GetRequiredPathByPage("/SocialWorkerRegistration/RegistrationComplete");

    // SWE registration change links
    public string SocialWorkerRegistrationDateOfBirthChange() => GetRequiredPathByPage("/SocialWorkerRegistration/SelectDateOfBirth", handler: "Change");
    public string SocialWorkerRegistrationSexAndGenderIdentityChange() => GetRequiredPathByPage("/SocialWorkerRegistration/SelectSexAndGenderIdentity", handler: "Change");
    public string SocialWorkerRegistrationEthnicGroupChange() => GetRequiredPathByPage("/SocialWorkerRegistration/SelectEthnicGroup/Index", handler: "Change");
    public string SocialWorkerRegistrationEthnicGroupWhiteChange() => GetRequiredPathByPage("/SocialWorkerRegistration/SelectEthnicGroup/White", handler: "Change");

    public string SocialWorkerRegistrationEthnicGroupMixedChange() =>
        GetRequiredPathByPage("/SocialWorkerRegistration/SelectEthnicGroup/MixedOrMultipleEthnicGroups", handler: "Change");

    public string SocialWorkerRegistrationEthnicGroupAsianChange() =>
        GetRequiredPathByPage("/SocialWorkerRegistration/SelectEthnicGroup/AsianOrAsianBritish", handler: "Change");

    public string SocialWorkerRegistrationEthnicGroupBlackChange() =>
        GetRequiredPathByPage("/SocialWorkerRegistration/SelectEthnicGroup/BlackAfricanCaribbeanOrBlackBritish", handler: "Change");

    public string SocialWorkerRegistrationEthnicGroupOtherChange() => GetRequiredPathByPage("/SocialWorkerRegistration/SelectEthnicGroup/OtherEthnicGroup", handler: "Change");
    public string SocialWorkerRegistrationSelectDisabilityChange() => GetRequiredPathByPage("/SocialWorkerRegistration/SelectDisability", handler: "Change");

    public string SocialWorkerRegistrationSelectSocialWorkEnglandRegistrationDateChange() =>
        GetRequiredPathByPage("/SocialWorkerRegistration/SelectSocialWorkEnglandRegistrationDate", handler: "Change");

    public string SocialWorkerRegistrationSelectHighestQualificationChange() =>
        GetRequiredPathByPage("/SocialWorkerRegistration/SelectHighestQualification", handler: "Change");

    public string SocialWorkerRegistrationSelectSocialWorkQualificationEndYearChange() =>
        GetRequiredPathByPage("/SocialWorkerRegistration/SelectSocialWorkQualificationEndYear", handler: "Change");

    public string SocialWorkerRegistrationSelectRouteIntoSocialWorkChange() => GetRequiredPathByPage("/SocialWorkerRegistration/SelectRouteIntoSocialWork", handler: "Change");

    protected internal abstract string GetRequiredPathByPage(
        string page,
        string? handler = null,
        object? routeValues = null,
        FragmentString? fragment = null
    );

    protected abstract string GetRequiredUriByPage(
        HttpContext httpContext,
        string page,
        string? handler = null,
        object? routeValues = null
    );
}

public class RoutingEcfLinkGenerator(
    IOptions<OidcConfiguration> oidcConfiguration,
    LinkGenerator linkGenerator
) : EcfLinkGenerator(oidcConfiguration)
{
    protected internal override string GetRequiredPathByPage(
        string page,
        string? handler = null,
        object? routeValues = null,
        FragmentString? fragment = null
    )
    {
        return linkGenerator.GetPathByPage(page, handler, values: routeValues, pathBase: PathString.Empty, fragment: fragment ?? FragmentString.Empty)
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
