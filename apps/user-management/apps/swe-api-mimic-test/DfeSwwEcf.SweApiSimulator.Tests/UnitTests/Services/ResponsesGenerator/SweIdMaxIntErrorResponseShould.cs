using System.Net;
using DfeSwwEcf.SweApiSimulator.Models;
using DfeSwwEcf.SweApiSimulator.Services.ResponsesGenerator;
using FluentAssertions;

namespace DfeSwwEcf.SweApiSimulator.Tests.UnitTests.Services.ResponsesGenerator;

public class SweIdMaxIntErrorResponseShould : ErrorResponsesTestsTestBase
{
    public SweIdMaxIntErrorResponseShould()
        : base(new SweIdMaxIntErrorResponse()) { }

    [Fact]
    public void WhenCalled_ReturnsSocialWorker()
    {
        // Arrange
        var expectedResponse = new SocialWorkerResponse
        {
            ErrorDetails = new()
            {
                HttpStatusCode = HttpStatusCode.InternalServerError,
                ErrorMessage =
                    "Internal server error: One or more errors occurred. (Value was either too large or too small for an Int32.)"
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
