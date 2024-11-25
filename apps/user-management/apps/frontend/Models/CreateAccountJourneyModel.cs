using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Models;

namespace Dfe.Sww.Ecf.Frontend.Models;

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
            Status = AccountStatus.Active,
            Email = AccountDetails?.Email,
            FirstName = AccountDetails?.FirstName,
            LastName = AccountDetails?.LastName,
            Types = AccountTypes,
            SocialWorkEnglandNumber = AccountDetails?.SocialWorkEnglandNumber
        };
    }
}
