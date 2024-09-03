using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Models;

namespace Dfe.Sww.Ecf.Frontend.Services.Interfaces;

public interface ISocialWorkEnglandService
{
    public Task<SocialWorker?> GetSocialWorkerAsync(int id);
}
