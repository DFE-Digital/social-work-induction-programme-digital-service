using System.Security.Cryptography;
using Microsoft.Extensions.Caching.Memory;

namespace Dfe.Sww.Ecf.Core.Services.Accounts;

public class OneLoginAccountLinkingService(
    IAccountsService accountsService,
    IMemoryCache memoryCache
) : IOneLoginAccountLinkingService
{
    private const string LinkingTokenCacheKey = "AccountLinkingToken";

    private static string GetLinkingTokenCacheKey(string linkingToken) =>
        $"{LinkingTokenCacheKey}-{linkingToken}";

    private async Task<bool> IsAccountIdValid(Guid accountId) => await accountsService.GetByIdAsync(accountId) is not null;

    private bool DoesLinkingTokenExist(string linkingToken) =>
        memoryCache.TryGetValue(GetLinkingTokenCacheKey(linkingToken), out _);

    public async Task<string> GetLinkingTokenForAccountId(Guid accountId)
    {
        if (!(await IsAccountIdValid(accountId)))
        {
            throw new InvalidOperationException("The account ID is not valid.");
        }

        var linkingToken = GenerateUniqueLinkingToken();
        AddLinkingTokenToCache(accountId, linkingToken);
        return linkingToken;
    }

    private string GenerateUniqueLinkingToken()
    {
        var generationAttempt = 0;
        while (generationAttempt < 5)
        {
            generationAttempt++;
            var linkingToken = RandomNumberGenerator.GetString(
                choices: "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789",
                length: 64
            );

            if (DoesLinkingTokenExist(linkingToken))
            {
                continue;
            }

            return linkingToken;
        }

        throw new InvalidOperationException("Could not generate a unique linking token.");
    }

    /**
     * Adds an account ID to the cache, keyed by the linking token
     */
    private void AddLinkingTokenToCache(Guid accountId, string linkingToken)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(
            TimeSpan.FromDays(3)
        );

        memoryCache.Set(GetLinkingTokenCacheKey(linkingToken), accountId, cacheEntryOptions);
    }
}
