namespace Dfe.Sww.Ecf.Core.Services.Accounts;

public interface IOneLoginAccountLinkingService
{
    public Task<string> GetLinkingTokenForAccountIdAsync(Guid accountId);

    public Task<Guid?> GetAccountIdForLinkingToken(string linkingToken);

    public Task InvalidateLinkingToken(string linkingToken);
}
