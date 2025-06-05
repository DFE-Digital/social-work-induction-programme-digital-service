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
    void Add(User user);

    /// <summary>
    /// Adds multiple accounts to the repository.
    /// </summary>
    /// <param name="accountsToAdd">The collection of accounts to add.</param>
    void AddRange(IReadOnlyCollection<User> accountsToAdd);

    /// <summary>
    /// Retrieves an account by its ID.
    /// </summary>
    /// <param name="id">The ID of the account to retrieve.</param>
    /// <returns>The account with the specified ID, or null if not found.</returns>
    User? GetById(Guid id);

    /// <summary>
    /// Retrieves all accounts in the repository.
    /// </summary>
    /// <returns>A collection of all accounts.</returns>
    IEnumerable<User> GetAll();

    /// <summary>
    /// Updates an existing account in the repository.
    /// </summary>
    /// <param name="account">The account to update.</param>
    void Update(User user);

    /// <summary>
    /// Checks if an account exists in the repository.
    /// </summary>
    /// <param name="id">The ID of the account to check.</param>
    /// <returns>True if the account exists, false otherwise.</returns>
    bool Exists(Guid id);
}
