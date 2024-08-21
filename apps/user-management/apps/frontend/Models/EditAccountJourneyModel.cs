using System.Collections.Immutable;

namespace Dfe.Sww.Ecf.Frontend.Models;

public class EditAccountJourneyModel(Account account)
{
    public Account Account { get; } = account;

    public ImmutableList<AccountType>? AccountTypes { get; set; } = account.Types;

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
            Email = AccountDetails?.Email,
            FirstName = AccountDetails?.FirstName,
            LastName = AccountDetails?.LastName,
            Types = AccountTypes
        };
    }
}
