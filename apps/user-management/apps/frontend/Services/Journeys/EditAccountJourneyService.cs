using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Configuration;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using Microsoft.Extensions.Options;

namespace Dfe.Sww.Ecf.Frontend.Services.Journeys;

public class EditAccountJourneyService(
    IHttpContextAccessor httpContextAccessor,
    IAccountService accountService,
    EcfLinkGenerator linkGenerator
) : IEditAccountJourneyService
{
    private static string EditAccountSessionKey(Guid id)
    {
        return "_editAccount-" + id;
    }

    private ISession Session =>
        httpContextAccessor.HttpContext?.Session ?? throw new NullReferenceException();

    private static KeyNotFoundException AccountNotFoundException(Guid id)
    {
        return new KeyNotFoundException("Account not found with ID " + id);
    }

    private async Task<EditAccountJourneyModel?> GetEditAccountJourneyModelAsync(Guid accountId)
    {
        Session.TryGet(
            EditAccountSessionKey(accountId),
            out EditAccountJourneyModel? editAccountJourneyModel
        );
        if (editAccountJourneyModel is not null) return editAccountJourneyModel;

        var account = await accountService.GetByIdAsync(accountId);
        if (account is null) return null;

        editAccountJourneyModel = new EditAccountJourneyModel(account)
        {
            StoredSocialWorkEnglandNumber = account.SocialWorkEnglandNumber
        };
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

    public async Task<string?> GetStoredSocialWorkEnglandNumberAsync(Guid accountId)
    {
        var editAccountJourneyModel = await GetEditAccountJourneyModelAsync(accountId);
        return editAccountJourneyModel?.StoredSocialWorkEnglandNumber;
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
        var account = await accountService.GetByIdAsync(accountId);
        if (account is null) throw AccountNotFoundException(accountId);

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

        await accountService.UpdateAsync(updatedAccount);

        await ResetEditAccountJourneyModelAsync(accountId);
        return updatedAccount;
    }

    public AccountChangeLinks GetAccountChangeLinks(Guid id, Guid? organisationId = null)
    {
        return new AccountChangeLinks
        {
            AccountTypesChangeLink = linkGenerator.ManageAccount.SelectUseCaseChange(id, organisationId),
            FirstNameChangeLink = linkGenerator.ManageAccount.EditAccountDetails(id, organisationId),
            MiddleNamesChangeLink = linkGenerator.ManageAccount.EditAccountDetails(id, organisationId),
            LastNameChangeLink = linkGenerator.ManageAccount.EditAccountDetails(id, organisationId),
            EmailChangeLink = linkGenerator.ManageAccount.EditAccountDetails(id, organisationId),
            SocialWorkEnglandNumberChangeLink = linkGenerator.ManageAccount.EditAccountDetails(id, organisationId),
            ProgrammeDatesChangeLink = linkGenerator.ManageAccount.SocialWorkerProgrammeDates(id, organisationId, "Update")
        };
    }
}
