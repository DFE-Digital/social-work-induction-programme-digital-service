namespace Dfe.Sww.Ecf.Frontend.Routing;

public class ManageOrganisationLinks(EcfLinkGenerator ecfLinkGenerator)
{
    public string Index(int? offset = 0, int? pageSize = 10) => ecfLinkGenerator.GetRequiredPathByPage("/ManageOrganisations/Index", routeValues: new { offset, pageSize });
    public string ViewOrganisationDetails(Guid id) => ecfLinkGenerator.GetRequiredPathByPage("/ManageOrganisations/OrganisationDetails", routeValues: new { id });

    // Create Links
    public string AddNew() => ecfLinkGenerator.GetRequiredPathByPage("/ManageOrganisations/EnterLocalAuthorityCode", handler: "new");
    public string EnterLocalAuthorityCode() => ecfLinkGenerator.GetRequiredPathByPage("/ManageOrganisations/EnterLocalAuthorityCode");
    public string ConfirmOrganisationDetails() => ecfLinkGenerator.GetRequiredPathByPage("/ManageOrganisations/ConfirmOrganisationDetails");
    public string CheckYourAnswers() => ecfLinkGenerator.GetRequiredPathByPage("/ManageOrganisations/CheckYourAnswers");
    public string AddPrimaryCoordinator() => ecfLinkGenerator.GetRequiredPathByPage("/ManageOrganisations/AddPrimaryCoordinator");

    // Create Change Links
    public string EnterLocalAuthorityCodeChange() => ecfLinkGenerator.GetRequiredPathByPage("/ManageOrganisations/EnterLocalAuthorityCode", handler: "Change");
    public string AddPrimaryCoordinatorChange() => ecfLinkGenerator.GetRequiredPathByPage("/ManageOrganisations/AddPrimaryCoordinator", handler: "Change");

    // Edit links
    public string EditPrimaryCoordinatorChangeType(Guid id) => ecfLinkGenerator.GetRequiredPathByPage("/ManageOrganisations/EditPrimaryCoordinatorChangeType", routeValues: new { id });
    public string ReplacePrimaryCoordinator(Guid id) => ecfLinkGenerator.GetRequiredPathByPage("/ManageOrganisations/EditPrimaryCoordinator", routeValues: new { id }, handler: "Replace");
    public string EditPrimaryCoordinator(Guid id) => ecfLinkGenerator.GetRequiredPathByPage("/ManageOrganisations/EditPrimaryCoordinator", routeValues: new { id });
    public string CheckYourAnswersEdit(Guid id) => ecfLinkGenerator.GetRequiredPathByPage("/ManageOrganisations/CheckYourAnswers", routeValues: new { id }, handler: "Edit");
    public string CheckYourAnswersReplace(Guid id) => ecfLinkGenerator.GetRequiredPathByPage("/ManageOrganisations/CheckYourAnswers", routeValues: new { id }, handler: "Replace");

    // Edit change links
    public string ReplacePrimaryCoordinatorChange(Guid id) => ecfLinkGenerator.GetRequiredPathByPage("/ManageOrganisations/EditPrimaryCoordinator", routeValues: new { id }, handler: "ReplaceChange");
}
