namespace Dfe.Sww.Ecf.Frontend.Routing;

public class ManageOrganisationLinks(EcfLinkGenerator ecfLinkGenerator)
{
    public string Index(int? offset = 0, int? pageSize = 10) => ecfLinkGenerator.GetRequiredPathByPage("/ManageOrganisations/Index", routeValues: new { offset, pageSize });
    public string AddNew() => ecfLinkGenerator.GetRequiredPathByPage("/ManageOrganisations/EnterLocalAuthorityCode", handler: "new");
    public string EnterLocalAuthorityCode() => ecfLinkGenerator.GetRequiredPathByPage("/ManageOrganisations/EnterLocalAuthorityCode");
    public string ConfirmOrganisationDetails() => ecfLinkGenerator.GetRequiredPathByPage("/ManageOrganisations/ConfirmOrganisationDetails");
    public string CheckYourAnswers() => ecfLinkGenerator.GetRequiredPathByPage("/ManageOrganisations/CheckYourAnswers");
    public string AddPrimaryCoordinator() => ecfLinkGenerator.GetRequiredPathByPage("/ManageOrganisations/AddPrimaryCoordinator");
    public string ViewOrganisationDetails(Guid id) => ecfLinkGenerator.GetRequiredPathByPage("/ManageOrganisations/OrganisationDetails",routeValues: new { id });

    // Change Links
    public string EnterLocalAuthorityCodeChange() => ecfLinkGenerator.GetRequiredPathByPage("/ManageOrganisations/EnterLocalAuthorityCode", handler: "Change");
    public string AddPrimaryCoordinatorChange() => ecfLinkGenerator.GetRequiredPathByPage("/ManageOrganisations/AddPrimaryCoordinator", handler: "Change");
}
