using Dfe.Sww.Ecf.Frontend.Views.Accounts;

namespace Dfe.Sww.Ecf.Frontend.Models;

public class CreateAccountJourneyModel
{
    public SelectUserTypeModel? UserType { get; set; }

    public AddUserDetailsModel? UserDetails { get; set; }

    public Account ToAccount()
    {
        return new Account
        {
            Id = Guid.NewGuid(),
            Status = AccountStatus.Active,
            Email = UserDetails?.Email,
            FirstName = UserDetails?.FirstName,
            LastName = UserDetails?.LastName,
            Types = UserType is null ? null : [UserType.AccountType]
        };
    }
}
