using System.Net;
using System.Security.Claims;
using System.Text.Json;
using Dfe.Sww.Ecf.Frontend.HttpClients.Authentication;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Options;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using RichardSzalay.MockHttp;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.HttpClient.AuthServiceClientTests;

public class AsyeSocialWorkerOperationsTests
{
    private readonly Mock<IOptions<AuthClientOptions>> _mockOptions;

    public AsyeSocialWorkerOperationsTests()
    {
        _mockOptions = new();
    }

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
    public async Task GetById_WhenErrorResponseReturned_ThrowsHttpRequestException()
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

    private AuthServiceClient BuildSut(MockHttpMessageHandler mockHttpMessageHandler)
    {
        var client = mockHttpMessageHandler.ToHttpClient();
        client.BaseAddress = new Uri("http://localhost");

        _mockOptions
            .Setup(x => x.Value)
            .Returns(
                new AuthClientOptions
                {
                    BaseUrl = "http://localhost",
                    ClientCredentials = new ClientCredentials
                    {
                        ClientId = string.Empty,
                        ClientSecret = string.Empty,
                        AccessTokenUrl = string.Empty
                    }
                }
            );

        var claims = new List<Claim>
        {
            new Claim("organisation_id", Guid.NewGuid().ToString())
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipal
        };
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
        var sut = new AuthServiceClient(client, mockHttpContextAccessor.Object);

        return sut;
    }

    private static (
        MockHttpMessageHandler MockHttpMessageHandler,
        MockedRequest MockedRequest
    ) GenerateMockClient(
        HttpStatusCode statusCode,
        HttpMethod httpMethod,
        object? response,
        string route
    )
    {
        using var mockHttp = new MockHttpMessageHandler();
        var request = mockHttp
            .When(httpMethod, route)
            .Respond(statusCode, "application/json", JsonSerializer.Serialize(response));

        return (mockHttp, request);
    }
}
