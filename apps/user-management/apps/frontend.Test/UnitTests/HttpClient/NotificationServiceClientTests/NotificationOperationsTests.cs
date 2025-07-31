using System.Net;
using System.Text.Json;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Models;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Options;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RichardSzalay.MockHttp;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.HttpClient.NotificationServiceClientTests;

public class NotificationOperationsTests
{
    private readonly Mock<ILogger<NotificationServiceClient>> _mockLogger = new();
    private readonly Mock<IOptions<NotificationClientOptions>> _mockOptions = new();
    private readonly NotificationRequestFaker _notificationRequestFaker = new();

    [Fact]
    public async Task GetById_SuccessfulRequest_ReturnsCorrectResponse()
    {
        // Arrange
        var route = "/SendEmail";
        var notificationRequest = _notificationRequestFaker.Generate();
        var notificationResponse = new NotificationResponse { StatusCode = HttpStatusCode.OK };

        var (mockHttp, request) = GenerateMockClient(
            HttpStatusCode.OK,
            notificationResponse,
            route
        );

        var sut = BuildSut(mockHttp, route);

        // Act
        var response = await sut.Notification.SendEmailAsync(notificationRequest);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<NotificationResponse>();
        response.Should().BeEquivalentTo(notificationResponse);

        mockHttp.GetMatchCount(request).Should().Be(1);
        mockHttp.VerifyNoOutstandingRequest();
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task GetById_WhenErrorResponseReturned_ReturnsNull()
    {
        // Arrange
        var route = "/GetSweWorker";
        var httpResponseCode = HttpStatusCode.BadRequest;
        var notificationRequest = _notificationRequestFaker.Generate();
        var notificationResponse = new NotificationResponse { StatusCode = httpResponseCode };

        var (mockHttp, request) = GenerateMockClient(
            HttpStatusCode.BadRequest,
            notificationResponse,
            route
        );

        var sut = BuildSut(mockHttp, route);

        // Act
        var response = await sut.Notification.SendEmailAsync(notificationRequest);

        // Assert
        response.Should().BeEquivalentTo(notificationResponse);

        mockHttp.GetMatchCount(request).Should().Be(1);
        mockHttp.VerifyNoOutstandingRequest();
        mockHttp.VerifyNoOutstandingExpectation();
    }

    private NotificationServiceClient BuildSut(
        MockHttpMessageHandler mockHttpMessageHandler,
        string route
    )
    {
        var client = mockHttpMessageHandler.ToHttpClient();
        client.BaseAddress = new Uri("http://localhost");

        _mockOptions
            .Setup(x => x.Value)
            .Returns(
                new NotificationClientOptions
                {
                    BaseUrl = "http://localhost",
                    Routes = new NotificationServiceRoutes { Notification = new NotificationRoutes { SendEmail = route } }
                }
            );

        var sut = new NotificationServiceClient(client, _mockOptions.Object, _mockLogger.Object);

        return sut;
    }

    private static (
        MockHttpMessageHandler MockHttpMessageHandler,
        MockedRequest MockedRequest
        ) GenerateMockClient(
            HttpStatusCode statusCode,
            NotificationResponse response,
            string route = "/api/Notification"
        )
    {
        using var mockHttp = new MockHttpMessageHandler();
        var request = mockHttp
            .When(HttpMethod.Post, route)
            .Respond(statusCode, "application/json", JsonSerializer.Serialize(response));

        return (mockHttp, request);
    }
}
