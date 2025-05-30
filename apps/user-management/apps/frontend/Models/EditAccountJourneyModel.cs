using System.Collections.Immutable;
using static Dfe.Sww.Ecf.Frontend.Models.AccountStatus;

namespace Dfe.Sww.Ecf.Frontend.Models;

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
            MiddleNames = account.MiddleNames,
            Email = account.Email,
            SocialWorkEnglandNumber = account.SocialWorkEnglandNumber,
            IsStaff = account.IsStaff
        };

    public bool? IsStaff { get; set; } = account.IsStaff;

    public Account ToAccount()
    {
        return new Account(Account)
        {
            Email = AccountDetails.Email,
            FirstName = AccountDetails.FirstName,
            MiddleNames = AccountDetails.MiddleNames,
            LastName = AccountDetails.LastName,
            SocialWorkEnglandNumber = AccountDetails.SocialWorkEnglandNumber,
            Types = AccountTypes,
            Status = AccountStatus
        };
    }
}
