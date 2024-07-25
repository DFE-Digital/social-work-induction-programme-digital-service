using DfeSwwEcf.SweApiSimulator.Services;
using DfeSwwEcf.SweApiSimulator.Services.Interfaces;
using Moq;

namespace DfeSwwEcf.SweApiSimulator.Tests.UnitTests.Services.SocialWorkerTests;

public abstract class SocialWorkerTestsTestBase
{
    private protected Mock<ISocialWorkerDataService> MockSocialWorkerDataService { get; set; }

    private protected ISocialWorkerService Sut { get; set; }

    public SocialWorkerTestsTestBase()
    {
        MockSocialWorkerDataService = new();

        Sut = new SocialWorkerService(MockSocialWorkerDataService.Object);
    }

    private protected void VerifyAllNoOtherCall()
    {
        MockSocialWorkerDataService.VerifyNoOtherCalls();
    }
}
