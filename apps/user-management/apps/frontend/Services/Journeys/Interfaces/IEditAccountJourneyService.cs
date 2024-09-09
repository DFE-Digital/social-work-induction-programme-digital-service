using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Models;

namespace Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;

public interface IEditAccountJourneyService
{
    bool IsAccountIdValid(Guid accountId);
    ImmutableList<AccountType>? GetAccountTypes(Guid accountId);
    AccountDetails GetAccountDetails(Guid accountId);
    bool? GetIsStaff(Guid accountId);
    void SetAccountDetails(Guid accountId, AccountDetails accountDetails);
    void SetAccountTypes(Guid accountId, IEnumerable<AccountType> accountTypes);
    void SetAccountStatus(Guid accountId, AccountStatus accountStatus);
    void SetIsStaff(Guid accountId, bool? isStaff);
    void ResetCreateAccountJourneyModel(Guid accountId);
    Account CompleteJourney(Guid accountId);
}
