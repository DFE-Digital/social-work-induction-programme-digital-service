namespace Dfe.Sww.Ecf.Frontend.Models;

public class RegisterSocialWorkerJourneyModel(Account account)
{
    public Account Account { get; } = account;

    public DateOnly? DateOfBirth { get; set; } = account.DateOfBirth;

    public UserSex? UserSex { get; set; } = account.UserSex;

    public GenderMatchesSexAtBirth? GenderMatchesSexAtBirth { get; set; } = account.GenderMatchesSexAtBirth;

    public Account ToAccount()
    {
        return new Account(Account)
        {
            DateOfBirth = DateOfBirth,
            UserSex = UserSex,
            GenderMatchesSexAtBirth = GenderMatchesSexAtBirth
        };
    }
}
