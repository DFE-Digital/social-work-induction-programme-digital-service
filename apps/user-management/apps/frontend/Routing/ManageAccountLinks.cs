namespace Dfe.Sww.Ecf.Frontend.Routing;

public class ManageAccountLinks(EcfLinkGenerator ecfLinkGenerator)
{
    public string Index(Guid? organisationId = null, int? offset = 0, int? pageSize = 10) => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/Index", routeValues: new { organisationId, offset, pageSize });
    public string ViewAccountDetails(Guid id, Guid? organisationId = null) => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/ViewAccountDetails", routeValues: new { id, organisationId });
    public string ViewAccountDetailsNew(Guid id, Guid? organisationId = null) => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/ViewAccountDetails", routeValues: new { id, organisationId }, handler: "New");

    // Create Links
    public string AddSomeoneNew(Guid? organisationId = null) => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/SelectAccountType", routeValues: new { organisationId }, handler: "New");
    public string SelectUseCase(Guid? organisationId = null) => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/SelectUseCase", routeValues: new { organisationId });
    public string SelectAccountType(Guid? organisationId = null) => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/SelectAccountType", routeValues: new { organisationId });
    public string AddAccountDetails(Guid? organisationId = null) => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/AddAccountDetails", routeValues: new { organisationId });
    public string ConfirmAccountDetails(Guid? organisationId = null) => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/ConfirmAccountDetails", routeValues: new { organisationId });

    // Create Change Links
    public string AddAccountDetailsChange(Guid? organisationId = null) => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/AddAccountDetails", routeValues: new { organisationId }, handler: "Change");

    public string AddAccountDetailsChangeFirstName(Guid? organisationId = null) =>
        ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/AddAccountDetails", handler: "Change", routeValues: new { organisationId }, fragment: new FragmentString("#FirstName"));

    public string AddAccountDetailsChangeMiddleNames(Guid? organisationId = null) =>
        ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/AddAccountDetails", handler: "Change", routeValues: new { organisationId }, fragment: new FragmentString("#MiddleNames"));

    public string AddAccountDetailsChangeLastName(Guid? organisationId = null) =>
        ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/AddAccountDetails", handler: "Change", routeValues: new { organisationId }, fragment: new FragmentString("#LastName"));

    public string AddAccountDetailsChangeEmail(Guid? organisationId = null) =>
        ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/AddAccountDetails", handler: "Change", routeValues: new { organisationId }, fragment: new FragmentString("#Email"));

    public string AddAccountDetailsChangeSocialWorkEnglandNumber(Guid? organisationId = null) => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/AddAccountDetails",
        handler: "Change",
        routeValues: new { organisationId },
        fragment: new FragmentString("#SocialWorkEnglandNumber"));

    public string SelectUseCaseChange(Guid? id = null, Guid? organisationId = null) => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/SelectUseCase", handler: "Change", routeValues: new { id, organisationId });
    public string SelectAccountTypeChange(Guid? organisationId = null) => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/SelectAccountType", handler: "Change", routeValues: new { organisationId });

    // Edit links
    public string EditAccountDetails(Guid id, Guid? organisationId = null) => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EditAccountDetails", routeValues: new { id, organisationId});

    public string ConfirmAccountDetailsUpdate(Guid id, Guid? organisationId = null) =>
        ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/ConfirmAccountDetails", handler: "Update", routeValues: new { id, organisationId });

    // Eligibility
    public string EligibilityInformation(Guid? organisationId = null) => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilityInformation", routeValues: new { organisationId });
    public string EligibilitySocialWorkEngland(Guid? organisationId = null) => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilitySocialWorkEngland", routeValues: new { organisationId });
    public string EligibilitySocialWorkEnglandChange(Guid? organisationId = null) => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilitySocialWorkEngland", handler: "Change", routeValues: new { organisationId });
    public string EligibilitySocialWorkEnglandAsyeDropout(Guid? organisationId = null) => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilitySocialWorkEnglandAsyeDropout", routeValues: new { organisationId });
    public string EligibilitySocialWorkEnglandDropout(Guid? organisationId = null) => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilitySocialWorkEnglandDropout", routeValues: new { organisationId });
    public string EligibilitySocialWorkEnglandDropoutChange(Guid? organisationId = null) => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilitySocialWorkEnglandDropout", handler: "Change", routeValues: new { organisationId });
    public string EligibilityStatutoryWork(Guid? organisationId = null) => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilityStatutoryWork", routeValues: new { organisationId });
    public string EligibilityStatutoryWorkChange(Guid? organisationId = null) => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilityStatutoryWork", handler: "Change", routeValues: new { organisationId });
    public string EligibilityStatutoryWorkDropout(Guid? organisationId = null) => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilityStatutoryWorkDropout", routeValues: new { organisationId });
    public string EligibilityStatutoryWorkDropoutChange(Guid? organisationId = null) => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilityStatutoryWorkDropout", handler: "Change", routeValues: new { organisationId });
    public string EligibilityAgencyWorker(Guid? organisationId = null) => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilityAgencyWorker", routeValues: new { organisationId });
    public string EligibilityAgencyWorkerChange(Guid? organisationId = null) => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilityAgencyWorker", handler: "Change", routeValues: new { organisationId });
    public string EligibilityQualification(Guid? organisationId = null) => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilityQualification", routeValues: new { organisationId });
    public string EligibilityQualificationChange(Guid? organisationId = null) => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilityQualification", handler: "Change", routeValues: new { organisationId });
    public string EligibilityFundingNotAvailable(Guid? organisationId = null) => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilityFundingNotAvailable", routeValues: new { organisationId });
    public string EligibilityFundingAvailable(Guid? organisationId = null) => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilityFundingAvailable", routeValues: new { organisationId });
    public string SocialWorkerProgrammeDates(Guid? organisationId = null) => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/SocialWorkerProgrammeDates", routeValues: new { organisationId });
    public string SocialWorkerProgrammeDatesChange(Guid? id = null, Guid? organisationId = null) => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/SocialWorkerProgrammeDates", handler: "Change", routeValues: new { id, organisationId });
}
