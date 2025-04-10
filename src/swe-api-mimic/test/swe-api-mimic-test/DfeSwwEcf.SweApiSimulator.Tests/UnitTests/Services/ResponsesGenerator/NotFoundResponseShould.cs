using System.Net;
using DfeSwwEcf.SweApiSimulator.Models;
using DfeSwwEcf.SweApiSimulator.Services.ResponsesGenerator;
using FluentAssertions;

namespace DfeSwwEcf.SweApiSimulator.Tests.UnitTests.Services.ResponsesGenerator;

public class NotFoundResponseShould : ErrorResponsesTestsTestBase
{
    public NotFoundResponseShould()
        : base(new NotFoundResponse()) { }

    [Fact]
    public void WhenCalled_ReturnsSocialWorker()
    {
        // Arrange
        var expectedResponse = new SocialWorkerResponse
        {
            ErrorDetails = new()
            {
                HttpStatusCode = HttpStatusCode.NotFound,
                ErrorMessage = "No SocialWorker found with this ID"
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
