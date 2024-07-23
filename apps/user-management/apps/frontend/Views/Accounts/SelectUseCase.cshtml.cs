using Dfe.Sww.Ecf.Frontend.Models;

namespace Dfe.Sww.Ecf.Frontend.Views.Accounts;

/// <summary>
/// Select Use Case Model
/// </summary>
public class SelectUseCaseModel
{
    /// <summary>
    /// Account Types
    /// </summary>
    public IList<AccountType>? AccountTypes { get; init; }
}
