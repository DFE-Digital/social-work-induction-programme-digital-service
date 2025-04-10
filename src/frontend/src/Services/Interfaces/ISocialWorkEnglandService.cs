using SocialWorkInductionProgramme.Frontend.HttpClients.SocialWorkEngland.Models;
using SocialWorkInductionProgramme.Frontend.Models.NameMatch;

namespace SocialWorkInductionProgramme.Frontend.Services.Interfaces;

public interface ISocialWorkEnglandService
{
    public Task<SocialWorker?> GetByIdAsync(string? sweId);

    public MatchResult? GetNameMatchScore(string firstName, string lastName, string sweName);
}
