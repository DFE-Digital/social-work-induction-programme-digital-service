using System.Net;
using DfeSwwEcf.SweApiSimulator.Models;
using FluentAssertions;
using NonIntSweIdResponse = DfeSwwEcf.SweApiSimulator.Services.ResponsesGenerator.NonIntSweIdResponse;

namespace DfeSwwEcf.SweApiSimulator.Tests.UnitTests.Services.ResponsesGenerator;

public class NonIntSweIdResponseShould : ErrorResponsesTestsTestBase
{
    public NonIntSweIdResponseShould()
        : base(new NonIntSweIdResponse()) { }

    [Fact]
    public void WhenCalled_ReturnsSocialWorker()
    {
        // Arrange
        var expectedResponse = new SocialWorkerResponse
        {
            ErrorDetails = new()
            {
                HttpStatusCode = HttpStatusCode.UnprocessableEntity,
                ErrorMessage = "Please enter valid integer value"
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
