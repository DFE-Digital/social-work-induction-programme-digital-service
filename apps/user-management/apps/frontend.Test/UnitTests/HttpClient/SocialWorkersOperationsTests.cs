using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using Dfe.Sww.Ecf.Frontend.HttpClients.Authentication;
using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland;
using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Models;
using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Options;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Polly;
using Polly.Retry;
using RichardSzalay.MockHttp;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.HttpClient;

public class SocialWorkersOperationsTests
{
    private const int RetryAttempts = 2;
    private const HttpStatusCode ErrorResponseStatusCode = HttpStatusCode.InternalServerError;

    private readonly SocialWorkerFaker _socialWorkerFaker;
    private readonly Mock<IOptions<SocialWorkEnglandClientOptions>> _mockOptions;
    private readonly ResiliencePipeline<HttpResponseMessage> _pipeline;

    public SocialWorkersOperationsTests()
    {
        _socialWorkerFaker = new();
        _mockOptions = new();

        _pipeline = new ResiliencePipelineBuilder<HttpResponseMessage>()
            .AddRetry(
                new RetryStrategyOptions<HttpResponseMessage>
                {
                    MaxRetryAttempts = RetryAttempts,
                    UseJitter = true,
                    ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                        .Handle<HttpRequestException>(ex => ex.InnerException is SocketException)
                        .HandleResult(response => response.StatusCode == ErrorResponseStatusCode),
                }
            )
            .Build();
    }

    [Fact]
    public async Task GetById_NoOptions_UsesFallbackRoute_ReturnsCorrectResponse()
    {
        // Arrange
        var swId = 1;
        var socialWorker = _socialWorkerFaker.GenerateWithId(swId);

        var (mockHttp, request) = GenerateMockClient(HttpStatusCode.OK, socialWorker);

        var sut = BuildSut(mockHttp);

        // Act
        var response = await sut.SocialWorkers.GetByIdAsync(swId);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<SocialWorker>();
        response.Should().BeEquivalentTo(socialWorker);

        mockHttp.GetMatchCount(request).Should().Be(1);
        mockHttp.VerifyNoOutstandingRequest();
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task GetById_WithOptions_UsesOptionsRoute_ReturnsCorrectResponse()
    {
        // Arrange
        var route = "/GetSweWorker";
        var swId = 1;
        var socialWorker = _socialWorkerFaker.GenerateWithId(swId);

        var (mockHttp, request) = GenerateMockClient(HttpStatusCode.OK, socialWorker, route);

        var sut = BuildSut(mockHttp, route);

        // Act
        var response = await sut.SocialWorkers.GetByIdAsync(swId);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<SocialWorker>();
        response.Should().BeEquivalentTo(socialWorker);

        mockHttp.GetMatchCount(request).Should().Be(1);
        mockHttp.VerifyNoOutstandingRequest();
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task GetById_WhenErrorResponseReturned_ReturnsNull()
    {
        // Arrange
        var route = "/GetSweWorker";
        var swId = 1;

        var (mockHttp, request) = GenerateMockClient(ErrorResponseStatusCode, null, route);

        var sut = BuildSut(mockHttp, route);

        // Act
        var response = await sut.SocialWorkers.GetByIdAsync(swId);

        // Assert
        response.Should().BeNull();

        mockHttp.GetMatchCount(request).Should().Be(RetryAttempts + 1);
        mockHttp.VerifyNoOutstandingRequest();
        mockHttp.VerifyNoOutstandingExpectation();
    }

    private SocialWorkEnglandClient BuildSut(
        MockHttpMessageHandler mockHttpMessageHandler,
        string? route = null
    )
    {
        var client = mockHttpMessageHandler.ToHttpClient();
        client.BaseAddress = new Uri("http://localhost");

        _mockOptions
            .Setup(x => x.Value)
            .Returns(
                new SocialWorkEnglandClientOptions
                {
                    BaseUrl = "http://localhost",
                    Routes = new() { SocialWorker = new() { GetById = route } },
                    ClientCredentials = new ClientCredentials
                    {
                        ClientId = string.Empty,
                        ClientSecret = string.Empty,
                        AccessTokenUrl = string.Empty
                    }
                }
            );

        var sut = new SocialWorkEnglandClient(client, _mockOptions.Object, _pipeline);

        return sut;
    }

    private static (
        MockHttpMessageHandler MockHttpMessageHandler,
        MockedRequest MockedRequest
    ) GenerateMockClient(
        HttpStatusCode statusCode,
        SocialWorker? response,
        string route = "/GetSocialWorkerById"
    )
    {
        using var mockHttp = new MockHttpMessageHandler();
        var request = mockHttp
            .When(HttpMethod.Get, route)
            .WithQueryString("swid", "1")
            .Respond(statusCode, "application/json", JsonSerializer.Serialize(response));

        return (mockHttp, request);
    }
}
