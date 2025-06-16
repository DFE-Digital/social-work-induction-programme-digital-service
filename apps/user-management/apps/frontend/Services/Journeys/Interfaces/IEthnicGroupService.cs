using Dfe.Sww.Ecf.Frontend.Models;

namespace Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;

public interface IEthnicGroupService
{
    Task<EthnicGroup?> GetEthnicGroupAsync(Guid accountId);
    Task SetEthnicGroupAsync(Guid accountId, EthnicGroup? ethnicGroup);
    Task<EthnicGroupWhite?> GetEthnicGroupWhiteAsync(Guid accountId);
    Task SetEthnicGroupWhiteAsync(Guid accountId, EthnicGroupWhite? ethnicGroupWhite);
    Task<string?> GetOtherEthnicGroupWhiteAsync(Guid accountId);
    Task SetOtherEthnicGroupWhiteAsync(Guid accountId, string? otherEthnicGroupWhite);
    Task<EthnicGroupMixed?> GetEthnicGroupMixedAsync(Guid accountId);
    Task SetEthnicGroupMixedAsync(Guid accountId, EthnicGroupMixed? ethnicGroupMixed);
    Task<string?> GetOtherEthnicGroupMixedAsync(Guid accountId);
    Task SetOtherEthnicGroupMixedAsync(Guid accountId, string? otherEthnicGroupMixed);
    Task<EthnicGroupAsian?> GetEthnicGroupAsianAsync(Guid accountId);
    Task SetEthnicGroupAsianAsync(Guid accountId, EthnicGroupAsian? ethnicGroupAsian);
    Task<string?> GetOtherEthnicGroupAsianAsync(Guid accountId);
    Task SetOtherEthnicGroupAsianAsync(Guid accountId, string? otherEthnicGroupAsian);
    Task<EthnicGroupBlack?> GetEthnicGroupBlackAsync(Guid accountId);
    Task SetEthnicGroupBlackAsync(Guid accountId, EthnicGroupBlack? ethnicGroupBlack);
    Task<string?> GetOtherEthnicGroupBlackAsync(Guid accountId);
    Task SetOtherEthnicGroupBlackAsync(Guid accountId, string? otherEthnicGroupBlack);
    Task<EthnicGroupOther?> GetEthnicGroupOtherAsync(Guid accountId);
    Task SetEthnicGroupOtherAsync(Guid accountId, EthnicGroupOther? ethnicGroupOther);
    Task<string?> GetOtherEthnicGroupOtherAsync(Guid accountId);
    Task SetOtherEthnicGroupOtherAsync(Guid accountId, string? otherEthnicGroupOther);
}
