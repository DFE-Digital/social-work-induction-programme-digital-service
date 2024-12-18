using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Models;
using Dfe.Sww.Ecf.Frontend.Models.NameMatch;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.NameMatch.Interfaces;

namespace Dfe.Sww.Ecf.Frontend.Services;

public class SocialWorkEnglandService(
    ISocialWorkEnglandClient client,
    ISocialWorkerValidatorService socialWorkerValidatorService
) : ISocialWorkEnglandService
{
    private readonly ISocialWorkEnglandClient _client = client;
    private readonly ISocialWorkerValidatorService _socialWorkerValidatorService =
        socialWorkerValidatorService;

    public async Task<SocialWorker?> GetByIdAsync(string? sweId)
    {
        if (string.IsNullOrWhiteSpace(sweId))
        {
            return null;
        }

        var isNumeric = int.TryParse(sweId.Where(char.IsDigit).ToArray(), out var id);
        if (!isNumeric)
        {
            return null;
        }

        return await _client.SocialWorkers.GetByIdAsync(id);
    }

    public MatchResult? GetNameMatchScore(string firstName, string lastName, string sweName)
    {
        return _socialWorkerValidatorService.ConvertToResult(firstName, lastName, sweName);
    }
}
