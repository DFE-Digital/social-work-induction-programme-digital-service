using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Repositories.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;

namespace Dfe.Sww.Ecf.Frontend.Services;

public class CreateAccountJourneyService(
    IHttpContextAccessor httpContextAccessor,
    IAccountRepository accountRepository
) : ICreateAccountJourneyService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IAccountRepository _accountRepository = accountRepository;

    private const string CreateAccountSessionKey = "_createAccount";

    private ISession Session =>
        _httpContextAccessor.HttpContext?.Session ?? throw new NullReferenceException();

    private CreateAccountJourneyModel GetCreateAccountJourneyModel()
    {
        Session.TryGet(
            CreateAccountSessionKey,
            out CreateAccountJourneyModel? createAccountJourneyModel
        );
        return createAccountJourneyModel ?? new CreateAccountJourneyModel();
    }

    public IList<AccountType>? GetAccountTypes()
    {
        var createAccountJourneyModel = GetCreateAccountJourneyModel();
        return createAccountJourneyModel.AccountTypes;
    }

    public AccountDetails? GetAccountDetails()
    {
        var createAccountJourneyModel = GetCreateAccountJourneyModel();
        return createAccountJourneyModel.AccountDetails;
    }

    public bool? GetIsStaff()
    {
        var createAccountJourneyModel = GetCreateAccountJourneyModel();
        return createAccountJourneyModel.IsStaff;
    }

    public void SetAccountDetails(AccountDetails accountDetails)
    {
        var createAccountJourneyModel = GetCreateAccountJourneyModel();
        createAccountJourneyModel.AccountDetails = accountDetails;
        SetCreateAccountJourneyModel(createAccountJourneyModel);
    }

    public void SetAccountTypes(IList<AccountType> accountTypes)
    {
        var createAccountJourneyModel = GetCreateAccountJourneyModel();
        createAccountJourneyModel.AccountTypes = accountTypes.ToImmutableList();
        SetCreateAccountJourneyModel(createAccountJourneyModel);
    }

    public void SetIsStaff(bool? isStaff)
    {
        var createAccountJourneyModel = GetCreateAccountJourneyModel();
        createAccountJourneyModel.IsStaff = isStaff;
        SetCreateAccountJourneyModel(createAccountJourneyModel);
    }

    public void ResetCreateAccountJourneyModel()
    {
        SetCreateAccountJourneyModel(new CreateAccountJourneyModel());
    }

    private void SetCreateAccountJourneyModel(CreateAccountJourneyModel createAccountJourneyModel)
    {
        Session.Set(CreateAccountSessionKey, createAccountJourneyModel);
    }

    public Account CompleteJourney()
    {
        var createAccountJourneyModel = GetCreateAccountJourneyModel();

        var account = createAccountJourneyModel.ToAccount();
        _accountRepository.Add(account);

        ResetCreateAccountJourneyModel();

        return account;
    }
}
