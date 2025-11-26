namespace Dfe.Sww.Ecf.Frontend.Routing;

public class ManageAccountLinks(EcfLinkGenerator ecfLinkGenerator)
{
    public string Index(Guid? organisationId = null, int? offset = 0, int? pageSize = 10)
    {
        return ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/Index", routeValues: new { organisationId, offset, pageSize });
    }

    public string ViewAccountDetails(Guid id, Guid? organisationId = null)
    {
        return ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/ViewAccountDetails", routeValues: new { id, organisationId });
    }

    public string ViewAccountDetailsNew(Guid id, Guid? organisationId = null)
    {
        return ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/ViewAccountDetails", routeValues: new { id, organisationId }, handler: "New");
    }

    // Create Links
    public string AddSomeoneNew(Guid? organisationId = null)
    {
        return ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/SelectAccountType", routeValues: new { organisationId }, handler: "New");
    }

    public string SelectUseCase(Guid? organisationId = null)
    {
        return ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/SelectUseCase", routeValues: new { organisationId });
    }

    public string SelectAccountType(Guid? organisationId = null)
    {
        return ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/SelectAccountType", routeValues: new { organisationId });
    }

    public string AddAccountDetails(Guid? organisationId = null)
    {
        return ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/AddAccountDetails", routeValues: new { organisationId });
    }

    public string ConfirmAccountDetails(Guid? organisationId = null)
    {
        return ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/ConfirmAccountDetails", routeValues: new { organisationId });
    }

    // Create Change Links
    public string AddAccountDetailsChange(Guid? organisationId = null)
    {
        return ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/AddAccountDetails", routeValues: new { organisationId }, handler: "Change");
    }

    public string AddAccountDetailsChangeFirstName(Guid? organisationId = null)
    {
        return ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/AddAccountDetails", "Change", new { organisationId }, new FragmentString("#FirstName"));
    }

    public string AddAccountDetailsChangeMiddleNames(Guid? organisationId = null)
    {
        return ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/AddAccountDetails", "Change", new { organisationId }, new FragmentString("#MiddleNames"));
    }

    public string AddAccountDetailsChangeLastName(Guid? organisationId = null)
    {
        return ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/AddAccountDetails", "Change", new { organisationId }, new FragmentString("#LastName"));
    }

    public string AddAccountDetailsChangeEmail(Guid? organisationId = null)
    {
        return ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/AddAccountDetails", "Change", new { organisationId }, new FragmentString("#Email"));
    }

    public string AddAccountDetailsChangeSocialWorkEnglandNumber(Guid? organisationId = null)
    {
        return ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/AddAccountDetails",
            "Change",
            new { organisationId },
            new FragmentString("#SocialWorkEnglandNumber"));
    }

    public string SelectUseCaseChange(Guid? id = null, Guid? organisationId = null)
    {
        return ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/SelectUseCase", "Change", new { id, organisationId });
    }

    public string SelectAccountTypeChange(Guid? organisationId = null)
    {
        return ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/SelectAccountType", "Change", new { organisationId });
    }

    // Edit links
    public string EditAccountDetails(Guid id, Guid? organisationId = null)
    {
        return ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EditAccountDetails", routeValues: new { id, organisationId });
    }

    public string ConfirmAccountDetailsUpdate(Guid id, Guid? organisationId = null)
    {
        return ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/ConfirmAccountDetails", "Update", new { id, organisationId });
    }

    // Eligibility
    public string EligibilityInformation(Guid? organisationId = null)
    {
        return ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilityInformation", routeValues: new { organisationId });
    }

    public string EligibilitySocialWorkEngland(Guid? organisationId = null)
    {
        return ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilitySocialWorkEngland", routeValues: new { organisationId });
    }

    public string EligibilitySocialWorkEnglandChange(Guid? organisationId = null)
    {
        return ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilitySocialWorkEngland", "Change", new { organisationId });
    }

    public string EligibilitySocialWorkEnglandAsyeDropout(Guid? organisationId = null)
    {
        return ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilitySocialWorkEnglandAsyeDropout", routeValues: new { organisationId });
    }

    public string EligibilitySocialWorkEnglandAsyeDropoutChange(Guid? organisationId = null)
    {
        return ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilitySocialWorkEnglandAsyeDropout", "Change", new { organisationId });
    }

    public string EligibilitySocialWorkEnglandDropout(Guid? organisationId = null)
    {
        return ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilitySocialWorkEnglandDropout", routeValues: new { organisationId });
    }

    public string EligibilitySocialWorkEnglandDropoutChange(Guid? organisationId = null)
    {
        return ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilitySocialWorkEnglandDropout", "Change", new { organisationId });
    }

    public string EligibilityStatutoryWork(Guid? organisationId = null)
    {
        return ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilityStatutoryWork", routeValues: new { organisationId });
    }

    public string EligibilityStatutoryWorkChange(Guid? organisationId = null)
    {
        return ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilityStatutoryWork", "Change", new { organisationId });
    }

    public string EligibilityStatutoryWorkDropout(Guid? organisationId = null)
    {
        return ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilityStatutoryWorkDropout", routeValues: new { organisationId });
    }

    public string EligibilityStatutoryWorkDropoutChange(Guid? organisationId = null)
    {
        return ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilityStatutoryWorkDropout", "Change", new { organisationId });
    }

    public string EligibilityAgencyWorker(Guid? organisationId = null)
    {
        return ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilityAgencyWorker", routeValues: new { organisationId });
    }

    public string EligibilityAgencyWorkerChange(Guid? organisationId = null)
    {
        return ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilityAgencyWorker", "Change", new { organisationId });
    }

    public string EligibilityQualification(Guid? organisationId = null)
    {
        return ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilityQualification", routeValues: new { organisationId });
    }

    public string EligibilityQualificationChange(Guid? organisationId = null)
    {
        return ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilityQualification", "Change", new { organisationId });
    }

    public string EligibilityFundingNotAvailable(Guid? organisationId = null)
    {
        return ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilityFundingNotAvailable", routeValues: new { organisationId });
    }

    public string EligibilityFundingAvailable(Guid? organisationId = null)
    {
        return ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/EligibilityFundingAvailable", routeValues: new { organisationId });
    }

    public string SocialWorkerProgrammeDates(Guid? id = null, Guid? organisationId = null, string? handler = null)
    {
        return ecfLinkGenerator.GetRequiredPathByPage("/ManageAccounts/SocialWorkerProgrammeDates", handler, new { id, organisationId });
    }
}
