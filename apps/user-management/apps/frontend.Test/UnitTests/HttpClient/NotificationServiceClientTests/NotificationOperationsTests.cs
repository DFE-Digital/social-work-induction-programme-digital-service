using System.Net;
using System.Text.Json;
using Dfe.Sww.Ecf.Frontend.Configuration;
using Dfe.Sww.Ecf.Frontend.Helpers;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Models;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Operations;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Options;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Microsoft.Extensions.Options;
using Moq;
using RichardSzalay.MockHttp;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.HttpClient.NotificationServiceClientTests;

public class NotificationOperationsTests
{
    private readonly FakeLogger<NotificationOperations> _fakeLogger = new();
    private readonly Mock<IOptions<FeatureFlags>> _mockFeatureFlags = new();
    private readonly NotificationRequestFaker _notificationRequestFaker = new();

    public NotificationOperationsTests()
    {
        _mockFeatureFlags.SetupGet(x => x.Value).Returns(new FeatureFlags());
    }

    [Fact]
    public async Task SendEmailAsync_SuccessfulRequest_ReturnsCorrectResponse()
    {
        // Arrange
        const string route = "/SendEmail";
        var notificationRequest = _notificationRequestFaker.Generate();
        var notificationResponse = new NotificationResponse { StatusCode = HttpStatusCode.OK };

        var (mockHttp, request) = GenerateMockClient(
            HttpStatusCode.OK,
            notificationResponse,
            route
        );

        var sut = BuildSut(mockHttp, route);

        // Act
        var response = await sut.SendEmailAsync(notificationRequest);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<NotificationResponse>();
        response.Should().BeEquivalentTo(notificationResponse);

        mockHttp.GetMatchCount(request).Should().Be(1);
        mockHttp.VerifyNoOutstandingRequest();
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task SendEmailAsync_WhenErrorResponseReturned_ErrorResponseAndLogs()
    {
        // Arrange
        const string route = "/SendEmail";
        const HttpStatusCode httpResponseCode = HttpStatusCode.BadRequest;
        var notificationRequest = _notificationRequestFaker.Generate();
        var notificationResponse = new NotificationResponse { StatusCode = httpResponseCode };

        var (mockHttp, request) = GenerateMockClient(
            HttpStatusCode.BadRequest,
            notificationResponse,
            route
        );

        var sut = BuildSut(mockHttp, route);

        // Act
        var response = await sut.SendEmailAsync(notificationRequest);

        // Assert
        response.Should().BeEquivalentTo(notificationResponse);
        _fakeLogger.LatestRecord.Level.Should().Be(LogLevel.Error);
        _fakeLogger.LatestRecord.Message.Should().Be($"[NotificationOperations] - Failed to send email with template ID {notificationRequest.TemplateId}");

        mockHttp.GetMatchCount(request).Should().Be(1);
        mockHttp.VerifyNoOutstandingRequest();
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task SendEmailAsync_WithPlusEmailStrippingFeatureFlagEnabled_StripsPlusTagFromEmail()
    {
        // Arrange
        const string route = "/SendEmail";
        var notificationRequest = _notificationRequestFaker.WithEmailAddress("test.email+foobar@email.com").Generate();
        var notificationResponse = new NotificationResponse { StatusCode = HttpStatusCode.OK };
        var featureFlags = new FeatureFlags { EnablePlusEmailStripping = true };

        using var mockHttp = new MockHttpMessageHandler();
        var expectedRequest = notificationRequest with
        {
            EmailAddress = "test.email@email.com"
        };
        mockHttp.Expect(HttpMethod.Post, route)
            .WithJsonContent(expectedRequest, new JsonSerializerOptions(JsonSerializerDefaults.Web) { Converters = { new BooleanConverter() } })
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(notificationResponse));

        var sut = BuildSut(mockHttp, route, featureFlags);

        // Act
        var response = await sut.SendEmailAsync(notificationRequest);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<NotificationResponse>();
        response.Should().BeEquivalentTo(notificationResponse);

        mockHttp.VerifyNoOutstandingRequest();
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task SendEmailAsync_WithPlusEmailStrippingFeatureFlagDisabled_DoesNotStripPlusTagFromEmail()
    {
        // Arrange
        const string route = "/SendEmail";
        var notificationRequest = _notificationRequestFaker.WithEmailAddress("test.email+foobar@email.com").Generate();
        var notificationResponse = new NotificationResponse { StatusCode = HttpStatusCode.OK };
        var featureFlags = new FeatureFlags { EnablePlusEmailStripping = false };

        using var mockHttp = new MockHttpMessageHandler();
        mockHttp.Expect(HttpMethod.Post, route)
            .WithJsonContent(notificationRequest, new JsonSerializerOptions(JsonSerializerDefaults.Web) { Converters = { new BooleanConverter() } })
            .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(notificationResponse));

        var sut = BuildSut(mockHttp, route, featureFlags);

        // Act
        var response = await sut.SendEmailAsync(notificationRequest);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<NotificationResponse>();
        response.Should().BeEquivalentTo(notificationResponse);

        mockHttp.VerifyNoOutstandingRequest();
        mockHttp.VerifyNoOutstandingExpectation();
    }

    private NotificationOperations BuildSut(
        MockHttpMessageHandler mockHttpMessageHandler,
        string route,
        FeatureFlags? featureFlags = null
    )
    {
        var client = mockHttpMessageHandler.ToHttpClient();
        client.BaseAddress = new Uri("http://localhost");

        var routes = new NotificationRoutes { SendEmail = route };
        if (featureFlags is not null) _mockFeatureFlags.SetupGet(x => x.Value).Returns(featureFlags);
        return new NotificationOperations(client, routes, _fakeLogger, _mockFeatureFlags.Object);
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
