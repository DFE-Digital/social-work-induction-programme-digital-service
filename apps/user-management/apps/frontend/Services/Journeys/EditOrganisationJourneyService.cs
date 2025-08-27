using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;

namespace Dfe.Sww.Ecf.Frontend.Services.Journeys;

public class EditOrganisationJourneyService(
    IHttpContextAccessor httpContextAccessor,
    IOrganisationService organisationService,
    IAccountService accountService) : IEditOrganisationJourneyService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IOrganisationService _organisationService = organisationService;
    private readonly IAccountService _accountService = accountService;

    private ISession Session =>
        _httpContextAccessor.HttpContext?.Session ?? throw new NullReferenceException();

    private static string EditOrganisationSessionKey(Guid id) => "_editOrganisation-" + id;

    private static KeyNotFoundException OrganisationNotFoundException(Guid id) =>
        new("Organisation not found with ID " + id);

    private async Task<EditOrganisationJourneyModel?> GetOrganisationJourneyModelAsync(Guid organisationId)
    {
        Session.TryGet(
            EditOrganisationSessionKey(organisationId),
            out EditOrganisationJourneyModel? editOrganisationJourneyModel
        );
        if (editOrganisationJourneyModel is not null)
        {
            return editOrganisationJourneyModel;
        }

        var organisation = await _organisationService.GetByIdAsync(organisationId);
        if (organisation?.PrimaryCoordinatorId is null)
        {
            return null;
        }

        var account = await _accountService.GetByIdAsync(organisation.PrimaryCoordinatorId.Value);
        var primaryCoordinator = account is not null ? AccountDetails.FromAccount(account) : null;
        if (primaryCoordinator is null)
        {
            return null;
        }

        editOrganisationJourneyModel = new EditOrganisationJourneyModel(organisation, primaryCoordinator);
        SetEditOrganisationJourneyModel(organisationId, editOrganisationJourneyModel);
        return editOrganisationJourneyModel;
    }

    private void SetEditOrganisationJourneyModel(Guid organisationId, EditOrganisationJourneyModel editOrganisationJourneyModel)
    {
        Session.Set(EditOrganisationSessionKey(organisationId), editOrganisationJourneyModel);
    }

    public async Task<PrimaryCoordinatorChangeType?> GetPrimaryCoordinatorChangeTypeAsync(Guid organisationId)
    {
        var editOrganisationJourneyModel = await GetOrganisationJourneyModelAsync(organisationId);
        return editOrganisationJourneyModel?.PrimaryCoordinatorChangeType;
    }

    public async Task SetPrimaryCoordinatorChangeTypeAsync(Guid organisationId, PrimaryCoordinatorChangeType? primaryCoordinatorChangeType)
    {
        var editOrganisationJourneyModel =
            await GetOrganisationJourneyModelAsync(organisationId)
            ?? throw OrganisationNotFoundException(organisationId);
        editOrganisationJourneyModel.PrimaryCoordinatorChangeType = primaryCoordinatorChangeType;
        SetEditOrganisationJourneyModel(organisationId, editOrganisationJourneyModel);
    }

    public async Task<Organisation?> GetOrganisationAsync(Guid organisationId)
    {
        var editOrganisationJourneyModel = await GetOrganisationJourneyModelAsync(organisationId);
        return editOrganisationJourneyModel?.Organisation;
    }

    public async Task SetOrganisationAsync(Guid organisationId, Organisation organisation)
    {
        var editOrganisationJourneyModel =
            await GetOrganisationJourneyModelAsync(organisationId)
            ?? throw OrganisationNotFoundException(organisationId);
        editOrganisationJourneyModel.Organisation = organisation;
        SetEditOrganisationJourneyModel(organisationId, editOrganisationJourneyModel);
    }

    public async Task<AccountDetails?> GetPrimaryCoordinatorAccountAsync(Guid organisationId)
    {
        var editOrganisationJourneyModel = await GetOrganisationJourneyModelAsync(organisationId);
        return editOrganisationJourneyModel?.PrimaryCoordinatorAccount;
    }

    public async Task SetPrimaryCoordinatorAccountAsync(Guid organisationId, AccountDetails account)
    {
        var editOrganisationJourneyModel =
            await GetOrganisationJourneyModelAsync(organisationId)
            ?? throw OrganisationNotFoundException(organisationId);
        editOrganisationJourneyModel.PrimaryCoordinatorAccount = account;
        SetEditOrganisationJourneyModel(organisationId, editOrganisationJourneyModel);
    }

    public void ResetEditOrganisationJourneyModel(Guid organisationId)
    {
        Session.Remove(EditOrganisationSessionKey(organisationId));
    }

    public async Task<Organisation?> CompleteJourneyAsync(Guid organisationId)
    {
        var editAccountJourneyModel = await GetOrganisationJourneyModelAsync(organisationId);

        var primaryCoordinator = editAccountJourneyModel?.PrimaryCoordinatorAccount;

        if (primaryCoordinator is null)
            throw new ArgumentNullException();

        var account = AccountDetails.ToAccount(primaryCoordinator);
        await _accountService.UpdateAsync(account);

        ResetEditOrganisationJourneyModel(organisationId);

        return new Organisation();
    }
}
