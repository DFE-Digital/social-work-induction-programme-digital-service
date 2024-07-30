using DfeSwwEcf.SweApiSimulator.Models;
using DfeSwwEcf.SweApiSimulator.Services.Interfaces;
using DfeSwwEcf.SweApiSimulator.Services.ResponsesGenerator.Interfaces;

namespace DfeSwwEcf.SweApiSimulator.Services;

/// <inheritdoc />
public class SocialWorkerService(
    ISocialWorkerDataService socialWorkerDataService,
    ISocialWorkerResponseFactory socialWorkerResponseFactory
) : ISocialWorkerService
{
    private readonly ISocialWorkerDataService _socialWorkerDataService = socialWorkerDataService;
    private readonly ISocialWorkerResponseFactory _socialWorkerResponseFactory =
        socialWorkerResponseFactory;

    /// <inheritdoc />
    public SocialWorkerResponse GetById(string? swId)
    {
        var socialWorker = _socialWorkerDataService.GetById(swId);

        var factory = _socialWorkerResponseFactory.Create(swId, socialWorker);

        var response = factory.MapResponse(socialWorker);

        return response;
    }
}
