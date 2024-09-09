using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Repositories.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;

namespace Dfe.Sww.Ecf.Frontend.Services.Journeys;

public class EditAccountJourneyService(
    IHttpContextAccessor httpContextAccessor,
    IAccountRepository accountRepository
) : IEditAccountJourneyService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IAccountRepository _accountRepository = accountRepository;

    private static string EditAccountSessionKey(Guid id) => "_editAccount-" + id;

    private ISession Session =>
        _httpContextAccessor.HttpContext?.Session ?? throw new NullReferenceException();

    private static KeyNotFoundException AccountNotFoundException(Guid id) =>
        new("Account not found with ID " + id);

    private EditAccountJourneyModel GetEditAccountJourneyModel(Guid accountId)
    {
        Session.TryGet(
            EditAccountSessionKey(accountId),
            out EditAccountJourneyModel? editAccountJourneyModel
        );
        if (editAccountJourneyModel is not null)
        {
            return editAccountJourneyModel;
        }

        var account = _accountRepository.GetById(accountId);
        if (account is null)
        {
            throw AccountNotFoundException(accountId);
        }

        editAccountJourneyModel = new EditAccountJourneyModel(account);
        return editAccountJourneyModel;
    }

    public bool IsAccountIdValid(Guid accountId) => _accountRepository.Exists(accountId);

    public ImmutableList<AccountType>? GetAccountTypes(Guid accountId)
    {
        var editAccountJourneyModel = GetEditAccountJourneyModel(accountId);
        return editAccountJourneyModel.AccountTypes;
    }

    public AccountDetails GetAccountDetails(Guid accountId)
    {
        var editAccountJourneyModel = GetEditAccountJourneyModel(accountId);
        return editAccountJourneyModel.AccountDetails;
    }

    public bool? GetIsStaff(Guid accountId)
    {
        var editAccountJourneyModel = GetEditAccountJourneyModel(accountId);
        return editAccountJourneyModel.IsStaff;
    }

    public void SetAccountDetails(Guid accountId, AccountDetails accountDetails)
    {
        var editAccountJourneyModel = GetEditAccountJourneyModel(accountId);
        editAccountJourneyModel.AccountDetails = accountDetails;
        SetEditAccountJourneyModel(accountId, editAccountJourneyModel);
    }

    public void SetAccountTypes(Guid accountId, IEnumerable<AccountType> accountTypes)
    {
        var editAccountJourneyModel = GetEditAccountJourneyModel(accountId);
        editAccountJourneyModel.AccountTypes = accountTypes.ToImmutableList();
        SetEditAccountJourneyModel(accountId, editAccountJourneyModel);
    }

    public void SetAccountStatus(Guid accountId, AccountStatus accountStatus)
    {
        var editAccountJourneyModel = GetEditAccountJourneyModel(accountId);
        editAccountJourneyModel.AccountStatus = accountStatus;
        SetEditAccountJourneyModel(accountId, editAccountJourneyModel);
    }

    public void SetIsStaff(Guid accountId, bool? isStaff)
    {
        var editAccountJourneyModel = GetEditAccountJourneyModel(accountId);
        editAccountJourneyModel.IsStaff = isStaff;
        SetEditAccountJourneyModel(accountId, editAccountJourneyModel);
    }

    public void ResetCreateAccountJourneyModel(Guid accountId)
    {
        var account = _accountRepository.GetById(accountId);
        if (account is null)
        {
            throw AccountNotFoundException(accountId);
        }
        Session.Remove(EditAccountSessionKey(accountId));
    }

    private void SetEditAccountJourneyModel(
        Guid accountId,
        EditAccountJourneyModel editAccountJourneyModel
    )
    {
        Session.Set(EditAccountSessionKey(accountId), editAccountJourneyModel);
    }

    public Account CompleteJourney(Guid accountId)
    {
        var editAccountJourneyModel = GetEditAccountJourneyModel(accountId);

        var updatedAccount = editAccountJourneyModel.ToAccount();
        _accountRepository.Update(updatedAccount);

        ResetCreateAccountJourneyModel(accountId);
        return updatedAccount;
    }
}
