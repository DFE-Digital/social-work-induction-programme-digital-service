using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Models;
using Dfe.Sww.Ecf.Frontend.Models.NameMatch;

namespace Dfe.Sww.Ecf.Frontend.Services.Interfaces;

public interface ISocialWorkEnglandService
{
    public Task<SocialWorker?> GetByIdAsync(string? sweId);

    public MatchResult? GetNameMatchScore(string? firstName, string? lastName, string? sweName);
}
