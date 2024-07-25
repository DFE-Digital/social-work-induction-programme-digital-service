using DfeSwwEcf.SweApiSimulator.Models;
using FluentAssertions;
using Moq;

namespace DfeSwwEcf.SweApiSimulator.Tests.UnitTests.Controllers.SocialWorkerTests;

public class GetByIdShould : SocialWorkerTestsTestBase
{
    [Fact]
    public void WhenCalled_ReturnSocialWorker()
    {
        // Arrange
        var socialWorkerId = 5604;

        var expectedResponse = new SocialWorker
        {
            RegistrationNumber = $"SW{socialWorkerId}",
            RegisteredName = "Emma Louise Doran",
            Status = "Registered",
            TownOfEmployment = "Workington",
            RegisteredFrom = new DateTime(2012, 8, 1),
            RegisteredUntil = new DateTime(2024, 11, 30),
            Registered = true
        };

        MockSocialWorkerService
            .Setup(x => x.GetById(socialWorkerId))
            .Returns(expectedResponse);

        // Act
        var response = Sut.GetById(socialWorkerId);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<SocialWorker>();
        response.Should().BeEquivalentTo(expectedResponse);

        MockSocialWorkerService.Verify(x => x.GetById(socialWorkerId), Times.Once);
        VerifyAllNoOtherCall();
    }
}
