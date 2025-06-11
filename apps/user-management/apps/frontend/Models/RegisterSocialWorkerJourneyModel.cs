namespace Dfe.Sww.Ecf.Frontend.Models;

public class RegisterSocialWorkerJourneyModel(Account account)
{
    public Account Account { get; } = account;

    public DateTime? DateOfBirth { get; set; } = account.DateOfBirth;

    public Account ToAccount()
    {
        return new Account(Account)
        {
            DateOfBirth = DateOfBirth
        };
    }
}
