using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Repositories.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Views.Accounts;

namespace Dfe.Sww.Ecf.Frontend.Services;

public class CreateAccountJourneyService(IHttpContextAccessor httpContextAccessor, IAccountRepository accountRepository) : ICreateAccountJourneyService
{
    private const string CreateAccountSessionKey = "_createAccount";

    private readonly IAccountRepository _accountRepository = accountRepository;

    private ISession Session => httpContextAccessor.HttpContext?.Session ?? throw new NullReferenceException();

    public CreateAccountJourneyModel GetCreateAccountJourneyModel()
    {
        Session.TryGet(CreateAccountSessionKey, out CreateAccountJourneyModel? createAccountJourneyModel);
        return createAccountJourneyModel ?? new CreateAccountJourneyModel();
    }

    public SelectUserTypeModel? GetUserType()
    {
        var createAccountJourneyModel = GetCreateAccountJourneyModel();
        return createAccountJourneyModel.UserType;
    }

    public AddUserDetailsModel? GetUserDetails()
    {
        var createAccountJourneyModel = GetCreateAccountJourneyModel();
        return createAccountJourneyModel.UserDetails;
    }

    public void SetUserDetails(AddUserDetailsModel userDetails)
    {
        var createAccountJourneyModel = GetCreateAccountJourneyModel();
        createAccountJourneyModel.UserDetails = userDetails;
        SetCreateAccountJourneyModel(createAccountJourneyModel);
    }

    public void SetAccountType(SelectUserTypeModel userType)
    {
        var createAccountJourneyModel = GetCreateAccountJourneyModel();
        createAccountJourneyModel.UserType = userType;
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
