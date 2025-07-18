using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;

namespace Dfe.Sww.Ecf.Frontend.Services.Journeys;

public class EditAccountJourneyService(
    IHttpContextAccessor httpContextAccessor,
    IAccountService accountService,
    EcfLinkGenerator linkGenerator
) : IEditAccountJourneyService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IAccountService _accountService = accountService;
    private readonly EcfLinkGenerator _linkGenerator = linkGenerator;

    private static string EditAccountSessionKey(Guid id) => "_editAccount-" + id;

    private ISession Session =>
        _httpContextAccessor.HttpContext?.Session ?? throw new NullReferenceException();

    private static KeyNotFoundException AccountNotFoundException(Guid id) =>
        new("Account not found with ID " + id);

    private async Task<EditAccountJourneyModel?> GetEditAccountJourneyModelAsync(Guid accountId)
    {
        Session.TryGet(
            EditAccountSessionKey(accountId),
            out EditAccountJourneyModel? editAccountJourneyModel
        );
        if (editAccountJourneyModel is not null)
        {
            return editAccountJourneyModel;
        }

        var account = await _accountService.GetByIdAsync(accountId);
        if (account is null)
        {
            return null;
        }

        editAccountJourneyModel = new EditAccountJourneyModel(account);
        SetEditAccountJourneyModel(accountId, editAccountJourneyModel);
        return editAccountJourneyModel;
    }

    public async Task<bool> IsAccountIdValidAsync(Guid accountId)
    {
        return await GetEditAccountJourneyModelAsync(accountId) is not null;
    }

    public async Task<IList<AccountType>?> GetAccountTypesAsync(Guid accountId)
    {
        var editAccountJourneyModel = await GetEditAccountJourneyModelAsync(accountId);
        return editAccountJourneyModel?.AccountDetails.Types;
    }

    public async Task<AccountDetails?> GetAccountDetailsAsync(Guid accountId)
    {
        var editAccountJourneyModel = await GetEditAccountJourneyModelAsync(accountId);
        return editAccountJourneyModel?.AccountDetails;
    }

    public async Task<bool?> GetIsStaffAsync(Guid accountId)
    {
        var editAccountJourneyModel = await GetEditAccountJourneyModelAsync(accountId);
        return editAccountJourneyModel?.IsStaff;
    }

    public async Task SetAccountDetailsAsync(Guid accountId, AccountDetails accountDetails)
    {
        var editAccountJourneyModel =
            await GetEditAccountJourneyModelAsync(accountId)
            ?? throw AccountNotFoundException(accountId);
        editAccountJourneyModel.AccountDetails = accountDetails;
        SetEditAccountJourneyModel(accountId, editAccountJourneyModel);
    }

    public async Task SetAccountTypesAsync(Guid accountId, IEnumerable<AccountType> accountTypes)
    {
        var editAccountJourneyModel =
            await GetEditAccountJourneyModelAsync(accountId)
            ?? throw AccountNotFoundException(accountId);
        editAccountJourneyModel.AccountDetails.Types = accountTypes.ToImmutableList();
        SetEditAccountJourneyModel(accountId, editAccountJourneyModel);
    }

    public async Task SetAccountStatusAsync(Guid accountId, AccountStatus accountStatus)
    {
        var editAccountJourneyModel =
            await GetEditAccountJourneyModelAsync(accountId)
            ?? throw AccountNotFoundException(accountId);
        editAccountJourneyModel.AccountStatus = accountStatus;
        SetEditAccountJourneyModel(accountId, editAccountJourneyModel);
    }

    public async Task SetIsStaffAsync(Guid accountId, bool? isStaff)
    {
        var editAccountJourneyModel =
            await GetEditAccountJourneyModelAsync(accountId)
            ?? throw AccountNotFoundException(accountId);
        editAccountJourneyModel.IsStaff = isStaff;
        SetEditAccountJourneyModel(accountId, editAccountJourneyModel);
    }

    public async Task ResetEditAccountJourneyModelAsync(Guid accountId)
    {
        var account = await _accountService.GetByIdAsync(accountId);
        if (account is null)
        {
            throw AccountNotFoundException(accountId);
        }

        Session.Remove(EditAccountSessionKey(accountId));
    }

    private void SetEditAccountJourneyModel(
        Guid accountId,
        EditAccountJourneyModel? editAccountJourneyModel
    )
    {
        Session.Set(EditAccountSessionKey(accountId), editAccountJourneyModel);
    }

    public async Task<Account> CompleteJourneyAsync(Guid accountId)
    {
        var editAccountJourneyModel =
            await GetEditAccountJourneyModelAsync(accountId)
            ?? throw AccountNotFoundException(accountId);

        var updatedAccount = editAccountJourneyModel.ToAccount();
        await _accountService.UpdateAsync(updatedAccount);

        await ResetEditAccountJourneyModelAsync(accountId);
        return updatedAccount;
    }

    public AccountChangeLinks GetAccountChangeLinks(Guid id)
    {
        return new AccountChangeLinks
        {
            AccountTypesChangeLink = _linkGenerator.SelectUseCaseChange(id),
            FirstNameChangeLink = _linkGenerator.EditAccountDetails(id),
            MiddleNamesChangeLink = _linkGenerator.EditAccountDetails(id),
            LastNameChangeLink = _linkGenerator.EditAccountDetails(id),
            EmailChangeLink = _linkGenerator.EditAccountDetails(id),
            SocialWorkEnglandNumberChangeLink = _linkGenerator.EditAccountDetails(id),
            ProgrammeDatesChangeLink = _linkGenerator.SocialWorkerProgrammeDatesChange(id)
        };
    }
}
