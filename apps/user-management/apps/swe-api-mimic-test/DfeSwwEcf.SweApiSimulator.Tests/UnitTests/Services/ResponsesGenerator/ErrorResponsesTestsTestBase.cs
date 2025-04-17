using DfeSwwEcf.SweApiSimulator.Services.ErrorResponses.Interfaces;
using DfeSwwEcf.SweApiSimulator.Tests.UnitTests.Helpers;

namespace DfeSwwEcf.SweApiSimulator.Tests.UnitTests.Services.ResponsesGenerator;

public abstract class ErrorResponsesTestsTestBase(ISocialWorkerResponse socialWorkerResponse)
{
    private protected SocialWorkerFaker SocialWorkerFaker { get; set; } = new();
    private protected ISocialWorkerResponse Sut { get; } = socialWorkerResponse;
}
