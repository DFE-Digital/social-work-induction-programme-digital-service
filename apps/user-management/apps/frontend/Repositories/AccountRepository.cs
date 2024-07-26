using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Repositories.Interfaces;

namespace Dfe.Sww.Ecf.Frontend.Repositories;

/// <inheritdoc />
public class AccountRepository : IAccountRepository
{
    private readonly List<Account> _accounts = [];

    /// <inheritdoc />
    public List<Account> GetAll()
    {
        return _accounts;
    }

    /// <inheritdoc />
    public void Add(Account account)
    {
        _accounts.Add(account);
    }

    /// <inheritdoc />
    public void AddRange(List<Account> accounts)
    {
        _accounts.AddRange(accounts);
    }
}
