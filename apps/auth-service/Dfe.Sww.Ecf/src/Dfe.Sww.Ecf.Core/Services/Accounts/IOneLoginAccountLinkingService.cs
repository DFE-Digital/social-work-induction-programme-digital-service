namespace Dfe.Sww.Ecf.Core.Services.Accounts;

public interface IOneLoginAccountLinkingService
{
    public Task<string> GetLinkingTokenForAccountIdAsync(Guid accountId);

    public Task<Guid?> GetAccountIdForLinkingToken(string linkingToken);

    public void InvalidateLinkingToken(string linkingToken);
}
