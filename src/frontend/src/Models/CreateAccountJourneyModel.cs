using System.Collections.Immutable;
using SocialWorkInductionProgramme.Frontend.HttpClients.SocialWorkEngland.Models;

namespace SocialWorkInductionProgramme.Frontend.Models;

public class CreateAccountJourneyModel
{
    public ImmutableList<AccountType>? AccountTypes { get; set; }

    public AccountDetails? AccountDetails { get; set; }

    public bool? IsStaff { get; set; }

    public SocialWorker? SocialWorkerDetails { get; set; }

    public Account ToAccount()
    {
        return new Account
        {
            Status =
                AccountTypes != null
                && AccountTypes.Contains(AccountType.EarlyCareerSocialWorker)
                && AccountDetails?.SocialWorkEnglandNumber is null
                    ? AccountStatus.PendingRegistration
                    : AccountStatus.Active,
            Email = AccountDetails?.Email,
            FirstName = AccountDetails?.FirstName,
            LastName = AccountDetails?.LastName,
            Types = AccountTypes,
            SocialWorkEnglandNumber = AccountDetails?.SocialWorkEnglandNumber
        };
    }
}
