using DfeSwwEcf.SweApiSimulator.Models;
using DfeSwwEcf.SweApiSimulator.Services.ResponsesGenerator;
using DfeSwwEcf.SweApiSimulator.Tests.UnitTests.Helpers;
using FluentAssertions;

namespace DfeSwwEcf.SweApiSimulator.Tests.UnitTests.Services.ResponsesGenerator;

public class ValidResponseShould : ErrorResponsesTestsTestBase
{
    public ValidResponseShould()
        : base(new ValidResponse()) { }

    [Fact]
    public void WhenCalled_ReturnsSocialWorker()
    {
        // Arrange
        var expectedResponse = new SocialWorkerResponse
        {
            SocialWorker = SocialWorkerFaker.Generate()
        };

        // Act
        var response = Sut.MapResponse(expectedResponse.SocialWorker);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<SocialWorkerResponse>();
        response.Should().BeEquivalentTo(expectedResponse);
    }
}
