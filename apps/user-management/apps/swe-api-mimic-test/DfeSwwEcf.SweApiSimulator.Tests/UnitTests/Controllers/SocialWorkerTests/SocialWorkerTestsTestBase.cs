using DfeSwwEcf.SweApiSimulator.Controllers;
using DfeSwwEcf.SweApiSimulator.Services.Interfaces;
using Moq;

namespace DfeSwwEcf.SweApiSimulator.Tests.UnitTests.Controllers.SocialWorkerTests;

public abstract class SocialWorkerTestsTestBase
{
    private protected Mock<ISocialWorkerService> MockSocialWorkerService { get; }

    private protected SocialWorkerController Sut { get; set; }

    public SocialWorkerTestsTestBase()
    {
        MockSocialWorkerService = new();

        Sut = new(MockSocialWorkerService.Object);
    }

    private protected void VerifyAllNoOtherCall()
    {
        MockSocialWorkerService.VerifyNoOtherCalls();
    }
}
