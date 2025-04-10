using System.Collections.Immutable;
using SocialWorkInductionProgramme.Frontend.Models;

namespace SocialWorkInductionProgramme.Frontend.Services.Journeys.Interfaces;

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
    Task ResetEditAccountJourneyModelAsync(Guid accountId);
    Task<Account> CompleteJourneyAsync(Guid accountId);
}
