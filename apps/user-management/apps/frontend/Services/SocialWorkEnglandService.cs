using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Models;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;

namespace Dfe.Sww.Ecf.Frontend.Services;

public class SocialWorkEnglandService(ISocialWorkEnglandClient client) : ISocialWorkEnglandService
{
    private readonly ISocialWorkEnglandClient _client = client;

    public async Task<SocialWorker?> GetSocialWorkerAsync(int id)
    {
        return await _client.SocialWorkers.GetByIdAsync(id);
    }
}
