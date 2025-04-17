using DfeSwwEcf.SweApiSimulator.Services;
using DfeSwwEcf.SweApiSimulator.Services.Interfaces;
using DfeSwwEcf.SweApiSimulator.Services.ResponsesGenerator.Interfaces;
using DfeSwwEcf.SweApiSimulator.Tests.UnitTests.Helpers;
using Moq;

namespace DfeSwwEcf.SweApiSimulator.Tests.UnitTests.Services.SocialWorkerTests;

public abstract class SocialWorkerTestsTestBase
{
    private protected Mock<ISocialWorkerDataService> MockSocialWorkerDataService { get; set; }

    private protected Mock<ISocialWorkerResponseFactory> MockSocialWorkerResponseFactory { get; set; }

    private protected SocialWorkerFaker SocialWorkerFaker { get; set; }

    private protected ISocialWorkerService Sut { get; set; }

    public SocialWorkerTestsTestBase()
    {
        MockSocialWorkerDataService = new();
        MockSocialWorkerResponseFactory = new();
        SocialWorkerFaker = new();

        Sut = new SocialWorkerService(
            MockSocialWorkerDataService.Object,
            MockSocialWorkerResponseFactory.Object
        );
    }

    private protected void VerifyAllNoOtherCall()
    {
        MockSocialWorkerDataService.VerifyNoOtherCalls();
        MockSocialWorkerResponseFactory.VerifyNoOtherCalls();
    }
}
