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

    public string Welcome() => GetRequiredPathByPage("/Welcome");

    public string Dashboard() => GetRequiredPathByPage("/Dashboard");

    public string ManageAccounts(int? offset = 0, int? pageSize = 10) =>
        GetRequiredPathByPage("/ManageAccounts/Index", routeValues: new { offset, pageSize });

    public string AddAccountDetails() => GetRequiredPathByPage("/ManageAccounts/AddAccountDetails");

    public string AddAccountDetailsChange() => GetRequiredPathByPage("/ManageAccounts/AddAccountDetails", handler: "Change");

    public string AddAccountDetailsChangeFirstName() =>
        GetRequiredPathByPage(
            "/ManageAccounts/AddAccountDetails",
            handler: "Change",
            fragment: new FragmentString("#FirstName")
        );

    public string AddAccountDetailsChangeMiddleNames() =>
        GetRequiredPathByPage(
            "/ManageAccounts/AddAccountDetails",
            handler: "Change",
            fragment: new FragmentString("#MiddleNames")
        );

    public string AddAccountDetailsChangeLastName() =>
        GetRequiredPathByPage(
            "/ManageAccounts/AddAccountDetails",
            handler: "Change",
            fragment: new FragmentString("#LastName")
        );

    public string AddAccountDetailsChangeEmail() =>
        GetRequiredPathByPage(
            "/ManageAccounts/AddAccountDetails",
            handler: "Change",
            fragment: new FragmentString("#Email")
        );

    public string AddAccountDetailsChangeSocialWorkEnglandNumber() =>
        GetRequiredPathByPage(
            "/ManageAccounts/AddAccountDetails",
            handler: "Change",
            fragment: new FragmentString("#SocialWorkEnglandNumber")
        );

    public string EditAccountDetails(Guid id) => GetRequiredPathByPage("/ManageAccounts/EditAccountDetails", routeValues: new { id });

    public string ConfirmAccountDetails() =>
        GetRequiredPathByPage("/ManageAccounts/ConfirmAccountDetails");

    public string ConfirmAccountDetailsUpdate(Guid id) =>
        GetRequiredPathByPage(
            "/ManageAccounts/ConfirmAccountDetails",
            handler: "Update",
            routeValues: new { id }
        );

    public string ViewAccountDetails(Guid id) => GetRequiredPathByPage("/ManageAccounts/ViewAccountDetails", routeValues: new { id });
    public string ViewAccountDetailsNew(Guid id) => GetRequiredPathByPage("/ManageAccounts/ViewAccountDetails", handler: "new", routeValues: new { id });
    public string SelectAccountType() => GetRequiredPathByPage("/ManageAccounts/SelectAccountType");
    public string SelectAccountTypeChange() => GetRequiredPathByPage("/ManageAccounts/SelectAccountType", handler: "Change");
    public string AddSomeoneNew() => GetRequiredPathByPage("/ManageAccounts/SelectAccountType", handler: "New");
    public string SelectUseCase() => GetRequiredPathByPage("/ManageAccounts/SelectUseCase");
    public string SelectUseCaseChange(Guid? id = null) => GetRequiredPathByPage("/ManageAccounts/SelectUseCase", handler: "Change", routeValues: new { id });
    public string AddExistingUser() => GetRequiredPathByPage("/ManageAccounts/AddExistingUser");
    public string EligibilityInformation() => GetRequiredPathByPage("/ManageAccounts/EligibilityInformation");
    public string EligibilitySocialWorkEngland() => GetRequiredPathByPage("/ManageAccounts/EligibilitySocialWorkEngland");
    public string EligibilitySocialWorkEnglandChange() => GetRequiredPathByPage("/ManageAccounts/EligibilitySocialWorkEngland", handler: "Change");
    public string EligibilitySocialWorkEnglandDropout() => GetRequiredPathByPage("/ManageAccounts/EligibilitySocialWorkEnglandDropout");
    public string EligibilitySocialWorkEnglandDropoutChange() => GetRequiredPathByPage("/ManageAccounts/EligibilitySocialWorkEnglandDropout", handler: "Change");
    public string EligibilityStatutoryWork() => GetRequiredPathByPage("/ManageAccounts/EligibilityStatutoryWork");
    public string EligibilityStatutoryWorkChange() => GetRequiredPathByPage("/ManageAccounts/EligibilityStatutoryWork", handler: "Change");
    public string EligibilityStatutoryWorkDropout() => GetRequiredPathByPage("/ManageAccounts/EligibilityStatutoryWorkDropout");
    public string EligibilityStatutoryWorkDropoutChange() => GetRequiredPathByPage("/ManageAccounts/EligibilityStatutoryWorkDropout", handler: "Change");
    public string EligibilityAgencyWorker() => GetRequiredPathByPage("/ManageAccounts/EligibilityAgencyWorker");
    public string EligibilityAgencyWorkerChange() => GetRequiredPathByPage("/ManageAccounts/EligibilityAgencyWorker", handler: "Change");
    public string EligibilityQualification() => GetRequiredPathByPage("/ManageAccounts/EligibilityQualification");
    public string EligibilityQualificationChange() => GetRequiredPathByPage("/ManageAccounts/EligibilityQualification", handler: "Change");
    public string EligibilityFundingNotAvailable() => GetRequiredPathByPage("/ManageAccounts/EligibilityFundingNotAvailable");
    public string EligibilityFundingAvailable() => GetRequiredPathByPage("/ManageAccounts/EligibilityFundingAvailable");
    public string SocialWorkerProgrammeDates() => GetRequiredPathByPage("/ManageAccounts/SocialWorkerProgrammeDates");

    public string SocialWorkerProgrammeDatesChange(Guid? id = null) =>
        GetRequiredPathByPage("/ManageAccounts/SocialWorkerProgrammeDates", handler: "Change", routeValues: new { id });

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
    public string SocialWorkerRegistrationSelectSocialWorkEnglandRegistrationDate() => GetRequiredPathByPage("/SocialWorkerRegistration/SelectSocialWorkEnglandRegistrationDate");
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
    public string SocialWorkerRegistrationEthnicGroupMixedChange() => GetRequiredPathByPage("/SocialWorkerRegistration/SelectEthnicGroup/MixedOrMultipleEthnicGroups", handler: "Change");
    public string SocialWorkerRegistrationEthnicGroupAsianChange() => GetRequiredPathByPage("/SocialWorkerRegistration/SelectEthnicGroup/AsianOrAsianBritish", handler: "Change");
    public string SocialWorkerRegistrationEthnicGroupBlackChange() => GetRequiredPathByPage("/SocialWorkerRegistration/SelectEthnicGroup/BlackAfricanCaribbeanOrBlackBritish", handler: "Change");
    public string SocialWorkerRegistrationEthnicGroupOtherChange() => GetRequiredPathByPage("/SocialWorkerRegistration/SelectEthnicGroup/OtherEthnicGroup", handler: "Change");
    public string SocialWorkerRegistrationSelectDisabilityChange() => GetRequiredPathByPage("/SocialWorkerRegistration/SelectDisability", handler: "Change");
    public string SocialWorkerRegistrationSelectSocialWorkEnglandRegistrationDateChange() => GetRequiredPathByPage("/SocialWorkerRegistration/SelectSocialWorkEnglandRegistrationDate", handler: "Change");
    public string SocialWorkerRegistrationSelectHighestQualificationChange() => GetRequiredPathByPage("/SocialWorkerRegistration/SelectHighestQualification", handler: "Change");
    public string SocialWorkerRegistrationSelectSocialWorkQualificationEndYearChange() => GetRequiredPathByPage("/SocialWorkerRegistration/SelectSocialWorkQualificationEndYear", handler: "Change");
    public string SocialWorkerRegistrationSelectRouteIntoSocialWorkChange() => GetRequiredPathByPage("/SocialWorkerRegistration/SelectRouteIntoSocialWork", handler: "Change");

    // Manage organisations links
    public string ManageOrganisations(int? offset = 0, int? pageSize = 10) => GetRequiredPathByPage("/ManageOrganisations/Index", routeValues: new { offset, pageSize });
    public string AddNewOrganisation() => GetRequiredPathByPage("/ManageOrganisations/EnterLocalAuthorityCode", handler: "new");
    public string EnterLocalAuthorityCode() => GetRequiredPathByPage("/ManageOrganisations/EnterLocalAuthorityCode");
    public string ConfirmOrganisationDetails() => GetRequiredPathByPage("/ManageOrganisations/ConfirmOrganisationDetails");
    public string AddPrimaryCoordinator() => GetRequiredPathByPage("/ManageOrganisations/AddPrimaryCoordinator");

    protected abstract string GetRequiredPathByPage(
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
    IWebHostEnvironment environment,
    IOptions<OidcConfiguration> oidcConfiguration,
    LinkGenerator linkGenerator
) : EcfLinkGenerator(environment, oidcConfiguration)
{
    protected override string GetRequiredPathByPage(
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
