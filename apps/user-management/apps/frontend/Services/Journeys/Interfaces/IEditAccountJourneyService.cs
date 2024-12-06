using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Models;

namespace Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;

public interface IEditAccountJourneyService
{
    Task<bool> IsAccountIdValidAsync(Guid accountId);
    Task<ImmutableList<AccountType>?> GetAccountTypesAsync(Guid accountId);
    Task<AccountDetails?> GetAccountDetailsAsync(Guid accountId);
    Task<bool?> GetIsStaffAsync(Guid accountId);
    Task SetAccountDetailsAsync(Guid accountId, AccountDetails accountDetails);
    Task SetAccountTypesAsync(Guid accountId, IEnumerable<AccountType> accountTypes);
    Task SetAccountStatusAsync(Guid accountId, AccountStatus accountStatus);
    Task SetIsStaffAsync(Guid accountId, bool? isStaff);
    Task ResetCreateAccountJourneyModelAsync(Guid accountId);
    Task<Account> CompleteJourneyAsync(Guid accountId);
}
