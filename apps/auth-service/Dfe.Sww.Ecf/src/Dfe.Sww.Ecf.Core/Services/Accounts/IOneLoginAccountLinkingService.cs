namespace Dfe.Sww.Ecf.Core.Services.Accounts;

public interface IOneLoginAccountLinkingService
{
    Task<string> GetLinkingTokenForAccountId(Guid accountId);
}
