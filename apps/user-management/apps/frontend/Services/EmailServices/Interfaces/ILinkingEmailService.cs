using System.Collections.Immutable;
using Dfe.Sww.Ecf.Frontend.Models;

namespace Dfe.Sww.Ecf.Frontend.Services.EmailServices.Interfaces;

public interface ILinkingEmailService
{
    Task<bool> LinkAccountAsync(
        AccountDetails? accountDetails,
        ImmutableList<AccountType>? accountTypes,
        string? coordinatorName,
        string? coordinatorEmail
    );

    Task<bool> UnlinkAccountAsync(
        AccountDetails? accountDetails,
        ImmutableList<AccountType>? accountTypes,
        string? coordinatorName,
        string? coordinatorEmail
    );
}
