using System.Net;
using DfeSwwEcf.SweApiSimulator.Models;
using DfeSwwEcf.SweApiSimulator.Services.ResponsesGenerator;
using FluentAssertions;

namespace DfeSwwEcf.SweApiSimulator.Tests.UnitTests.Services.ResponsesGenerator;

public class InvalidResponseShould : ErrorResponsesTestsTestBase
{
    public InvalidResponseShould()
        : base(new InvalidErrorResponse()) { }

    [Fact]
    public void WhenCalled_ReturnsSocialWorker()
    {
        // Arrange
        var expectedResponse = new SocialWorkerResponse
        {
            ErrorDetails = new()
            {
                HttpStatusCode = HttpStatusCode.OK,
                ErrorMessage = "Invalid Request"
            }
        };

        // Act
        var response = Sut.MapResponse();

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<SocialWorkerResponse>();
        response.Should().BeEquivalentTo(expectedResponse);
    }
}
