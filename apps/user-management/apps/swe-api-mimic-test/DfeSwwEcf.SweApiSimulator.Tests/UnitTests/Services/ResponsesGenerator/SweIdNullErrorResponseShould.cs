using System.Net;
using DfeSwwEcf.SweApiSimulator.Models;
using DfeSwwEcf.SweApiSimulator.Services.ResponsesGenerator;
using FluentAssertions;

namespace DfeSwwEcf.SweApiSimulator.Tests.UnitTests.Services.ResponsesGenerator;

public class SweIdNullErrorResponseShould : ErrorResponsesTestsTestBase
{
    public SweIdNullErrorResponseShould()
        : base(new SweIdNullErrorResponse()) { }

    [Fact]
    public void WhenCalled_ReturnsSocialWorker()
    {
        // Arrange
        var expectedResponse = new SocialWorkerResponse
        {
            ErrorDetails = new()
            {
                HttpStatusCode = HttpStatusCode.BadRequest,
                ErrorMessage = "Please provide non-null value"
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
