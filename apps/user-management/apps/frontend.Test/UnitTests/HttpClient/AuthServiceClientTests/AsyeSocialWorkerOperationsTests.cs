using System.Net;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.HttpClient.AuthServiceClientTests;

public class AsyeSocialWorkerOperationsTests : AuthServiceClientTestBase
{
    [Fact]
    public async Task ExistsAsync_SuccessfulRequest_ReturnsCorrectResponse()
    {
        // Arrange
        var socialWorkerId = "SW123";
        var expectedResponse = true;
        var route = $"/api/AsyeSocialWorker/{socialWorkerId}";

        var (mockHttp, request) = GenerateMockClient(
            HttpStatusCode.OK,
            HttpMethod.Get,
            expectedResponse,
            route
        );

        var sut = BuildSut(mockHttp);

        // Act
        var response = await sut.AsyeSocialWorker.ExistsAsync(socialWorkerId);

        // Assert
        response.Should().Be(expectedResponse);

        mockHttp.GetMatchCount(request).Should().Be(1);
        mockHttp.VerifyNoOutstandingRequest();
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task ExistsAsync_WhenErrorResponseReturned_ThrowsHttpRequestException()
    {
        // Arrange
        var socialWorkerId = "SW123";
        var route = $"/api/AsyeSocialWorker/{socialWorkerId}";

        var (mockHttp, request) = GenerateMockClient(
            HttpStatusCode.BadRequest,
            HttpMethod.Get,
            null,
            route
        );

        var sut = BuildSut(mockHttp);

        // Act
        var exception = await Assert.ThrowsAsync<HttpRequestException>(
            () => sut.AsyeSocialWorker.ExistsAsync(socialWorkerId)
        );

        // Assert
        exception.Message.Should().Be($"Failed check for social worker ID {socialWorkerId}.");

        mockHttp.GetMatchCount(request).Should().Be(1);
        mockHttp.VerifyNoOutstandingRequest();
        mockHttp.VerifyNoOutstandingExpectation();
    }
}
