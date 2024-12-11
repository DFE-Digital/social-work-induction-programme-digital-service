using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Models;

namespace Dfe.Sww.Ecf.Frontend.Services.Interfaces;

public interface IEmailService
{
    Task<bool> PauseAccountAsync(
        AccountDetails? accountDetails,
        ImmutableList<AccountType>? accountTypes,
        string? coordinatorName,
        string? coordinatorEmail
    );

    Task<bool> UnpauseAccountAsync(
        AccountDetails? accountDetails,
        ImmutableList<AccountType>? accountTypes,
        string? coordinatorName,
        string? coordinatorEmail
    );
}
