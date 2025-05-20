using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Models;

namespace Dfe.Sww.Ecf.Frontend.Models;

public class CreateAccountJourneyModel
{
    public ImmutableList<AccountType>? AccountTypes { get; set; }

    public AccountDetails? AccountDetails { get; set; }

    public bool? IsStaff { get; set; }

    public SocialWorker? SocialWorkerDetails { get; set; }

    public int? ExternalUserId { get; set; }

    public bool? IsRegisteredWithSocialWorkEngland { get; set; }

    public bool? IsStatutoryWorker { get; set; }

    public bool? IsAgencyWorker { get; set; }

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
            SocialWorkEnglandNumber = AccountDetails?.SocialWorkEnglandNumber,
            ExternalUserId = ExternalUserId
        };
    }
}
