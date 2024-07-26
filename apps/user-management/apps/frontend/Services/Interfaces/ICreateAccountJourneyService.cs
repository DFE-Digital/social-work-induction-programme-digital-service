using Dfe.Sww.Ecf.Frontend.Models;

namespace Dfe.Sww.Ecf.Frontend.Services.Interfaces;

public interface ICreateAccountJourneyService
{
    IList<AccountType>? GetAccountTypes();

    AccountDetails? GetAccountDetails();

    void SetAccountDetails(AccountDetails accountDetails);

    void SetAccountTypes(IList<AccountType> accountTypes);

    Account CompleteJourney();
}
