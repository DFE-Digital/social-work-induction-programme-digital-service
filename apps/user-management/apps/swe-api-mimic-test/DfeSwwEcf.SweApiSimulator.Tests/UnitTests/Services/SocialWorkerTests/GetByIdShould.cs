using DfeSwwEcf.SweApiSimulator.Models;
using FluentAssertions;
using Moq;

namespace DfeSwwEcf.SweApiSimulator.Tests.UnitTests.Services.SocialWorkerTests;

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
            RegisteredName = "Ralph Cormier",
            Status = "Registered",
            TownOfEmployment = "Workington",
            RegisteredFrom = new DateTime(2012, 8, 1),
            RegisteredUntil = new DateTime(2024, 11, 30),
            Registered = true
        };

        MockSocialWorkerDataService
            .Setup(x => x.GetById(socialWorkerId))
            .Returns(expectedResponse);

        // Act
        var response = Sut.GetById(socialWorkerId);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<SocialWorker>();
        response.Should().BeEquivalentTo(expectedResponse);

        MockSocialWorkerDataService.Verify(x => x.GetById(socialWorkerId), Times.Once);
        VerifyAllNoOtherCall();
    }
}
