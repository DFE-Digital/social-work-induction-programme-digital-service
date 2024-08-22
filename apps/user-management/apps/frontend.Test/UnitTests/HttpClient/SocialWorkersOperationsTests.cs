using System.Text.Json;
using Dfe.Sww.Ecf.Frontend.HttpClients.Authentication;
using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland;
using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Models;
using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Options;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using RichardSzalay.MockHttp;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.HttpClient;

public class SocialWorkersOperationsTests
{
    private readonly Mock<IOptions<SocialWorkEnglandClientOptions>> _mockOptions;
    private readonly SocialWorkerFaker _socialWorkerFaker;

    public SocialWorkersOperationsTests()
    {
        _mockOptions = new();
        _socialWorkerFaker = new();
    }

    [Fact]
    public async Task GetById_NoOptions_UsesFallbackRoute_ReturnsCorrectResponse()
    {
        // Arrange
        var swId = 1;
        var socialWorker = _socialWorkerFaker.GenerateWithId(swId);

        var (mockHttp, request) = GenerateMockClient(socialWorker);

        var sut = BuildSut(mockHttp);

        // Act
        var response = await sut.SocialWorkers.GetById(swId);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<SocialWorker>();
        response.Should().BeEquivalentTo(socialWorker);

        mockHttp.GetMatchCount(request).Should().Be(1);
    }

    [Fact]
    public async Task GetById_WithOptions_UsesOptionsRoute_ReturnsCorrectResponse()
    {
        // Arrange
        var route = "/GetSweWorker";
        var swId = 1;
        var socialWorker = _socialWorkerFaker.GenerateWithId(swId);

        var (mockHttp, request) = GenerateMockClient(socialWorker, route);

        var sut = BuildSut(mockHttp, route);

        // Act
        var response = await sut.SocialWorkers.GetById(swId);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<SocialWorker>();
        response.Should().BeEquivalentTo(socialWorker);

        mockHttp.GetMatchCount(request).Should().Be(1);
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

        var sut = new SocialWorkEnglandClient(client, _mockOptions.Object);

        return sut;
    }

    private static (
        MockHttpMessageHandler MockHttpMessageHandler,
        MockedRequest MockedRequest
    ) GenerateMockClient(SocialWorker socialWorker, string route = "/GetSocialWorkerById")
    {
        using var mockHttp = new MockHttpMessageHandler();
        var request = mockHttp
            .When(HttpMethod.Get, route)
            .WithQueryString("swid", "1")
            .Respond("application/json", JsonSerializer.Serialize(socialWorker));

        return (mockHttp, request);
    }
}
