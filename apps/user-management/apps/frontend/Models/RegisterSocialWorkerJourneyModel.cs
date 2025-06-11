namespace Dfe.Sww.Ecf.Frontend.Models;

public class RegisterSocialWorkerJourneyModel(Account account)
{
    public Account Account { get; } = account;

    public DateTime? DateOfBirth { get; set; }

    public Account ToAccount()
    {
        return new Account(Account)
        {
            DateOfBirth = DateOfBirth
        };
    }
}
