using System.Security.Cryptography;
using Dfe.Sww.Ecf.Core.DataStore.Postgres;
using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;

namespace Dfe.Sww.Ecf.Core.Services.Accounts;

public class OneLoginAccountLinkingService(IAccountsService accountsService, EcfDbContext dbContext)
    : IOneLoginAccountLinkingService
{
    private async Task<bool> IsAccountIdValid(Guid accountId) =>
        await accountsService.GetByIdAsync(accountId) is not null;

    private async Task<bool> DoesLinkingTokenExist(string token)
    {
        var linkingToken = await dbContext.LinkingTokens.FirstOrDefaultAsync(x =>
            x.Token.Equals(token)
        );

        return linkingToken != null;
    }

    public async Task<Guid?> GetAccountIdForLinkingToken(string token)
    {
        var linkingToken = await dbContext.LinkingTokens.FirstOrDefaultAsync(x =>
            x.Token.Equals(token)
        );

        return linkingToken?.PersonId;
    }

    public async Task<string> GetLinkingTokenForAccountIdAsync(Guid accountId)
    {
        if (!await IsAccountIdValid(accountId))
        {
            throw new InvalidOperationException("The account ID is not valid.");
        }

        var token = await GenerateUniqueLinkingToken();
        var linkingToken = new LinkingToken { PersonId = accountId, Token = token };
        await dbContext.LinkingTokens.AddAsync(linkingToken);

        return linkingToken.Token;
    }

    private async Task<string> GenerateUniqueLinkingToken()
    {
        var generationAttempt = 0;
        while (generationAttempt < 5)
        {
            generationAttempt++;
            var linkingToken = RandomNumberGenerator.GetString(
                choices: "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789",
                length: 64
            );

            if (await DoesLinkingTokenExist(linkingToken))
            {
                continue;
            }

            return linkingToken;
        }

        throw new InvalidOperationException("Could not generate a unique linking token.");
    }

    public void InvalidateLinkingToken(string token)
    {
        var linkingToken = dbContext.LinkingTokens.FirstOrDefault(x => x.Token.Equals(token));
        if (linkingToken is null)
        {
            return;
        }

        dbContext.LinkingTokens.Remove(linkingToken);
        dbContext.SaveChanges();
    }
}
