namespace Dfe.Sww.Ecf.Frontend.Models;

public class CreateAccountJourneyModel
{
    public IList<AccountType>? AccountTypes { get; set; }

    public AccountDetails? AccountDetails { get; set; }

    public Account ToAccount()
    {
        return new Account
        {
            Id = Guid.NewGuid(),
            Status = AccountStatus.Active,
            Email = AccountDetails?.Email,
            FirstName = AccountDetails?.FirstName,
            LastName = AccountDetails?.LastName,
            Types = AccountTypes
        };
    }
}
