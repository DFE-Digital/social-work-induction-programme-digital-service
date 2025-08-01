namespace Dfe.Sww.Ecf.Frontend.Routing;

public class ManageAccountLinks(EcfLinkGenerator ecfLinkGenerator)
{
    public string Index(int? offset = 0, int? pageSize = 10) => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/Index", routeValues: new { offset, pageSize });
    public string ViewAccountDetails(Guid id) => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/ViewAccountDetails", routeValues: new { id });
    public string ViewAccountDetailsNew(Guid id) => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/ViewAccountDetails", routeValues: new { id }, handler: "New");

    // Create Links
    public string AddSomeoneNew() => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/SelectAccountType", handler: "New");
    public string SelectUseCase() => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/SelectUseCase");
    public string SelectAccountType() => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/SelectAccountType");
    public string AddAccountDetails() => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/AddAccountDetails");
    public string ConfirmAccountDetails() => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/ConfirmAccountDetails");

    // Create Change Links
    public string AddAccountDetailsChange() => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/AddAccountDetails", handler: "Change");

    public string AddAccountDetailsChangeFirstName() =>
        ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/AddAccountDetails", handler: "Change", fragment: new FragmentString("#FirstName"));

    public string AddAccountDetailsChangeMiddleNames() =>
        ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/AddAccountDetails", handler: "Change", fragment: new FragmentString("#MiddleNames"));

    public string AddAccountDetailsChangeLastName() =>
        ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/AddAccountDetails", handler: "Change", fragment: new FragmentString("#LastName"));

    public string AddAccountDetailsChangeEmail() =>
        ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/AddAccountDetails", handler: "Change", fragment: new FragmentString("#Email"));

    public string AddAccountDetailsChangeSocialWorkEnglandNumber() => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/AddAccountDetails", handler: "Change",
        fragment: new FragmentString("#SocialWorkEnglandNumber"));

    public string SelectUseCaseChange(Guid? id = null) => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/SelectUseCase", handler: "Change", routeValues: new { id });
    public string SelectAccountTypeChange() => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/SelectAccountType", handler: "Change");

    // Edit links
    public string EditAccountDetails(Guid id) => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EditAccountDetails", routeValues: new { id });

    public string ConfirmAccountDetailsUpdate(Guid id) =>
        ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/ConfirmAccountDetails", handler: "Update", routeValues: new { id });

    // Eligibility
    public string EligibilityInformation() => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilityInformation");
    public string EligibilitySocialWorkEngland() => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilitySocialWorkEngland");
    public string EligibilitySocialWorkEnglandChange() => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilitySocialWorkEngland", handler: "Change");
    public string EligibilitySocialWorkEnglandDropout() => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilitySocialWorkEnglandDropout");
    public string EligibilitySocialWorkEnglandDropoutChange() => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilitySocialWorkEnglandDropout", handler: "Change");
    public string EligibilityStatutoryWork() => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilityStatutoryWork");
    public string EligibilityStatutoryWorkChange() => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilityStatutoryWork", handler: "Change");
    public string EligibilityStatutoryWorkDropout() => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilityStatutoryWorkDropout");
    public string EligibilityStatutoryWorkDropoutChange() => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilityStatutoryWorkDropout", handler: "Change");
    public string EligibilityAgencyWorker() => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilityAgencyWorker");
    public string EligibilityAgencyWorkerChange() => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilityAgencyWorker", handler: "Change");
    public string EligibilityQualification() => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilityQualification");
    public string EligibilityQualificationChange() => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilityQualification", handler: "Change");
    public string EligibilityFundingNotAvailable() => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilityFundingNotAvailable");
    public string EligibilityFundingAvailable() => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilityFundingAvailable");
    public string SocialWorkerProgrammeDates() => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/SocialWorkerProgrammeDates");
    public string SocialWorkerProgrammeDatesChange(Guid? id = null) => ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/SocialWorkerProgrammeDates", handler: "Change", routeValues: new { id });
}
