using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Views.Accounts;

namespace Dfe.Sww.Ecf.Frontend.Services.Interfaces;

public interface ICreateAccountJourneyService
{
    CreateAccountJourneyModel GetCreateAccountJourneyModel();

    IList<AccountType>? GetAccountTypes();

    AddAccountDetailsModel? GetAccountDetails();

    void SetAccountDetails(AddAccountDetailsModel accountDetails);

    void SetAccountTypes(IList<AccountType> accountTypes);

    void ResetCreateAccountJourneyModel();

    Account CompleteJourney();
}
