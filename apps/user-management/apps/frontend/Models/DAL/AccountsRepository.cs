namespace Dfe.Sww.Ecf.Frontend.Models.DAL;

public class AccountsRepository
{
    private readonly List<Account> _accounts = [];

    public List<Account> GetAll()
    {
        return _accounts;
    }

    public void Add(Account account)
    {
        _accounts.Add(account);
    }

    public void AddRange(List<Account> accounts)
    {
        _accounts.AddRange(accounts);
    }

    public int Count()
    {
        return _accounts.Count;
    }
}
