using DfeSwwEcf.SweApiSimulator.Models;
using DfeSwwEcf.SweApiSimulator.Services.ResponsesGenerator;
using FluentAssertions;
using Moq;

namespace DfeSwwEcf.SweApiSimulator.Tests.UnitTests.Services.SocialWorkerTests;

public class GetByIdShould : SocialWorkerTestsTestBase
{
    [Fact]
    public void WhenCalled_ReturnSocialWorker()
    {
        // Arrange
        var expectedResponse = new SocialWorkerResponse
        {
            SocialWorker = SocialWorkerFaker.Generate()
        };

        MockSocialWorkerDataService
            .Setup(x => x.GetById(expectedResponse.SocialWorker.Id))
            .Returns(expectedResponse.SocialWorker);

        MockSocialWorkerResponseFactory
            .Setup(x => x.Create(expectedResponse.SocialWorker.Id, expectedResponse.SocialWorker))
            .Returns(new ValidResponse());

        // Act
        var response = Sut.GetById(expectedResponse.SocialWorker.Id);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<SocialWorkerResponse>();
        response.Should().BeEquivalentTo(expectedResponse);

        MockSocialWorkerDataService.Verify(
            x => x.GetById(expectedResponse.SocialWorker.Id),
            Times.Once
        );
        MockSocialWorkerResponseFactory.Verify(
            x => x.Create(expectedResponse.SocialWorker.Id, expectedResponse.SocialWorker),
            Times.Once
        );
        VerifyAllNoOtherCall();
    }
}
