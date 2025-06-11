namespace Dfe.Sww.Ecf.Frontend.Models;

public class RegisterSocialWorkerJourneyModel(Account account)
{
    public Account Account { get; } = account;

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

    public DateTime? DateOfBirth { get; set; }

    public Account ToAccount()
    {
        return new Account(Account)
        {
            DateOfBirth = DateOfBirth
        };
    }
}
