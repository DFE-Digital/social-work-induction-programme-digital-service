using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;

namespace Dfe.Sww.Ecf.Frontend.Services.Journeys;

public class EditOrganisationJourneyService(
    IHttpContextAccessor httpContextAccessor) : IEditOrganisationJourneyService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    private ISession Session =>
        _httpContextAccessor.HttpContext?.Session ?? throw new NullReferenceException();

    private const string EditOrganisationSessionKey = "_editOrganisation";

    private EditOrganisationJourneyModel GetOrganisationJourneyModel()
    {
        Session.TryGet(
            EditOrganisationSessionKey,
            out EditOrganisationJourneyModel? editOrganisationJourneyModel
        );
        return editOrganisationJourneyModel ?? new EditOrganisationJourneyModel();
    }

    private void SetEditOrganisationJourneyModel(EditOrganisationJourneyModel editOrganisationJourneyModel)
    {
        Session.Set(EditOrganisationSessionKey, editOrganisationJourneyModel);
    }

    public PrimaryCoordinatorChangeType? GetPrimaryCoordinatorChangeType()
    {
        var editOrganisationJourneyModel = GetOrganisationJourneyModel();
        return editOrganisationJourneyModel.PrimaryCoordinatorChangeType;
    }

    public void SetPrimaryCoordinatorChangeType(PrimaryCoordinatorChangeType? primaryCoordinatorChangeType)
    {
        var editOrganisationJourneyModel = GetOrganisationJourneyModel();
        editOrganisationJourneyModel.PrimaryCoordinatorChangeType = primaryCoordinatorChangeType;
        SetEditOrganisationJourneyModel(editOrganisationJourneyModel);
    }

    public Organisation? GetOrganisation()
    {
        var editOrganisationJourneyModel = GetOrganisationJourneyModel();
        return editOrganisationJourneyModel.Organisation;
    }

    public void SetOrganisation(Organisation organisation)
    {
        var editOrganisationJourneyModel = GetOrganisationJourneyModel();
        editOrganisationJourneyModel.Organisation = organisation;
        SetEditOrganisationJourneyModel(editOrganisationJourneyModel);
    }

    public Account? GetPrimaryCoordinatorAccount()
    {
        var editOrganisationJourneyModel = GetOrganisationJourneyModel();
        return editOrganisationJourneyModel?.PrimaryCoordinatorAccount;
    }

    public void SetPrimaryCoordinatorAccount(Account account)
    {
        var editOrganisationJourneyModel = GetOrganisationJourneyModel();
        editOrganisationJourneyModel.PrimaryCoordinatorAccount = account;
        SetEditOrganisationJourneyModel(editOrganisationJourneyModel);
    }

    public void ResetEditOrganisationJourneyModel()
    {
        Session.Remove(EditOrganisationSessionKey);
    }

    public async Task<Organisation?> CompleteJourneyAsync()
    {
        var editAccountJourneyModel = GetOrganisationJourneyModel();

        // TODO update org here

        ResetEditOrganisationJourneyModel();

        return new Organisation();
    }
}
