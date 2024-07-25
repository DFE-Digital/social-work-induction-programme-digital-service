using DfeSwwEcf.SweApiSimulator.Models;
using DfeSwwEcf.SweApiSimulator.Services.Interfaces;

namespace DfeSwwEcf.SweApiSimulator.Services;

/// <inheritdoc />
public class SocialWorkerService(ISocialWorkerDataService socialWorkerDataService) : ISocialWorkerService
{
    private readonly ISocialWorkerDataService _socialWorkerDataService = socialWorkerDataService;

    /// <inheritdoc />
    public SocialWorker? GetById(int swId)
    {
        var socialWorker = _socialWorkerDataService.GetById(swId);

        return socialWorker;
    }
}
