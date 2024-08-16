using Dfe.Sww.Ecf.Frontend.Models;

namespace Dfe.Sww.Ecf.Frontend.Repositories.Interfaces;

/// <summary>
/// Account Repository
/// </summary>
public interface IAccountRepository
{
    /// <summary>
    /// Adds a new account to the repository.
    /// </summary>
    /// <param name="account">The account to add.</param>
    void Add(Account account);

    /// <summary>
    /// Adds multiple accounts to the repository.
    /// </summary>
    /// <param name="accountsToAdd">The collection of accounts to add.</param>
    void AddRange(IReadOnlyCollection<Account> accountsToAdd);

    /// <summary>
    /// Retrieves an account by its ID.
    /// </summary>
    /// <param name="id">The ID of the account to retrieve.</param>
    /// <returns>The account with the specified ID, or null if not found.</returns>
    Account? GetById(Guid id);

    /// <summary>
    /// Retrieves all accounts in the repository.
    /// </summary>
    /// <returns>A collection of all accounts.</returns>
    IEnumerable<Account> GetAll();

    /// <summary>
    /// Updates an existing account in the repository.
    /// </summary>
    /// <param name="account">The account to update.</param>
    void Update(Account account);
}
