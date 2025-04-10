using System.Collections.Immutable;
using static SocialWorkInductionProgramme.Frontend.Models.AccountStatus;

namespace SocialWorkInductionProgramme.Frontend.Models;

public class EditAccountJourneyModel(Account account)
{
    public Account Account { get; } = account;

    public ImmutableList<AccountType>? AccountTypes { get; set; } = account.Types;
    public AccountStatus? AccountStatus { get; set; } = account.Status;

    public AccountDetails AccountDetails { get; set; } =
        new()
        {
            FirstName = account.FirstName,
            LastName = account.LastName,
            Email = account.Email,
            SocialWorkEnglandNumber = account.SocialWorkEnglandNumber
        };

    public bool? IsStaff { get; set; } = account.IsStaff;

    public Account ToAccount()
    {
        return new Account(Account)
        {
            Email = AccountDetails.Email,
            FirstName = AccountDetails.FirstName,
            LastName = AccountDetails.LastName,
            SocialWorkEnglandNumber = AccountDetails.SocialWorkEnglandNumber,
            Types = AccountTypes,
            Status = AccountStatus switch
            {
                _
                    => AccountTypes != null
                    && AccountTypes.Contains(AccountType.EarlyCareerSocialWorker)
                    && AccountDetails.SocialWorkEnglandNumber is null
                        ? PendingRegistration
                        : Active
            }
        };
    }
}
