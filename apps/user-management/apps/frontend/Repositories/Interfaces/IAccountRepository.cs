using Dfe.Sww.Ecf.Frontend.Models;

namespace Dfe.Sww.Ecf.Frontend.Repositories.Interfaces;

/// <summary>
/// Account Repository
/// </summary>
public interface IAccountRepository
{
    /// <summary>
    /// Get all accounts
    /// </summary>
    /// <returns>A list of all accounts</returns>
    List<Account> GetAll();

    /// <summary>
    /// Get account by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>The account associated with the passed id</returns>
    Account? GetById(Guid? id);

    /// <summary>
    /// Add Account
    /// </summary>
    /// <param name="account"></param>
    void Add(Account account);

    /// <summary>
    /// Add list of accounts
    /// </summary>
    /// <param name="accounts"></param>
    void AddRange(List<Account> accounts);
}
