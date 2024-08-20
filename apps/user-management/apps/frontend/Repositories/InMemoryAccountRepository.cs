using System.Collections.Concurrent;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Repositories.Interfaces;

namespace Dfe.Sww.Ecf.Frontend.Repositories;

/// <summary>
/// In-memory account repository implementation to CRUD operations without a backing service/database.
/// </summary>
/// <remarks>
/// TODO: Replace with an implementation that calls another service to retrieve accounts.
/// </remarks>
public class InMemoryAccountRepository : IAccountRepository
{
    private readonly ConcurrentDictionary<Guid, Account> _accounts = new();

    /// <inheritdoc />
    public void Add(Account account)
    {
        ArgumentNullException.ThrowIfNull(account);

        if (!_accounts.TryAdd(account.Id, account))
        {
            throw new InvalidOperationException("Account with the same ID already exists.");
        }
    }

    /// <inheritdoc />
    public void AddRange(IReadOnlyCollection<Account> accountsToAdd)
    {
        ArgumentNullException.ThrowIfNull(accountsToAdd);
        if (accountsToAdd.Count == 0)
            return;

        var existingKeys = _accounts.Keys;
        var newAccounts = accountsToAdd.ToList();

        var existingIds = newAccounts
            .Where(account => existingKeys.Contains(account.Id))
            .Select(account => account.Id)
            .ToList();

        if (existingIds.Count > 0)
        {
            var errorMessage = string.Join(
                ", ",
                existingIds.Select(id => $"Account with ID {id} already exists.")
            );
            throw new InvalidOperationException(errorMessage);
        }

        foreach (
            var account in newAccounts.Where(account => !_accounts.TryAdd(account.Id, account))
        )
        {
            // This shouldn't happen after the above existingIds check
            // but just in case we'll make sure to throw an error.
            throw new InvalidOperationException($"Account with ID {account.Id} already exists.");
        }
    }

    /// <inheritdoc />
    public Account? GetById(Guid id)
    {
        _accounts.TryGetValue(id, out var account);
        return account;
    }

    /// <inheritdoc />
    public IEnumerable<Account> GetAll() => _accounts.Values.ToList();

    /// <inheritdoc />
    public void Update(Account account)
    {
        ArgumentNullException.ThrowIfNull(account);

        if (!_accounts.TryUpdate(account.Id, account, _accounts[account.Id]))
        {
            throw new KeyNotFoundException("Account not found.");
        }
    }

    /// <inheritdoc />
    public bool Exists(Guid id) => _accounts.ContainsKey(id);
}
