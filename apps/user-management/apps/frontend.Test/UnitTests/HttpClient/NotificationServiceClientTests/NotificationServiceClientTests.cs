using Dfe.Sww.Ecf.Frontend.Configuration;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Operations;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Options;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Microsoft.Extensions.Options;
using Moq;
using RichardSzalay.MockHttp;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.HttpClient.NotificationServiceClientTests;

public class NotificationServiceClientTests
{
    private readonly FakeLogger<NotificationServiceClient> _fakeLogger = new();
    private readonly Mock<IOptions<FeatureFlags>> _mockFeatureFlags = new();
    private readonly Mock<IOptions<NotificationClientOptions>> _mockOptions = new();

    [Fact]
    public void Constructor_CreatesOperations()
    {
        // Arrange
        using var mockHttp = new MockHttpMessageHandler();
        var httpClient = mockHttp.ToHttpClient();

        _mockOptions
            .Setup(x => x.Value)
            .Returns(new NotificationClientOptions
            {
                BaseUrl = "http://localhost",
                Routes = new NotificationServiceRoutes { Notification = new NotificationRoutes { SendEmail = "" } }
            });

        // Act
        var serviceClient = new NotificationServiceClient(httpClient, _mockOptions.Object, _fakeLogger, _mockFeatureFlags.Object);

        // Assert
        serviceClient.Should().NotBeNull();
        serviceClient.Notification.Should().NotBeNull();
        serviceClient.Notification.Should().BeOfType<NotificationOperations>();
    }

    [Fact]
    public void Constructor_WithFunctionKey_AddsHeaderAndLogsDebug()
    {
        // Arrange
        const string functionKey = "test-key";
        using var mockHttp = new MockHttpMessageHandler();
        var client = mockHttp.ToHttpClient();

        _mockOptions
            .Setup(x => x.Value)
            .Returns(new NotificationClientOptions
            {
                BaseUrl = "http://localhost",
                Routes = new NotificationServiceRoutes { Notification = new NotificationRoutes { SendEmail = "" } },
                FunctionKey = functionKey
            });

        // Act
        _ = new NotificationServiceClient(client, _mockOptions.Object, _fakeLogger, _mockFeatureFlags.Object);

        // Assert
        client.DefaultRequestHeaders.Contains("x-functions-key").Should().BeTrue();
        client.DefaultRequestHeaders.GetValues("x-functions-key").First().Should().Be(functionKey);
        _fakeLogger.LatestRecord.Level.Should().Be(LogLevel.Debug);
        _fakeLogger.LatestRecord.Message.Should().Be("Adding x-functions-key header to all requests");
    }

    [Fact]
    public void Constructor_WithoutFunctionKey_LogsWarning()
    {
        // Arrange
        using var mockHttp = new MockHttpMessageHandler();
        var client = mockHttp.ToHttpClient();

        _mockOptions
            .Setup(x => x.Value)
            .Returns(new NotificationClientOptions
            {
                BaseUrl = "http://localhost",
                Routes = new NotificationServiceRoutes { Notification = new NotificationRoutes { SendEmail = "" } },
                FunctionKey = null
            });

        // Act
        _ = new NotificationServiceClient(client, _mockOptions.Object, _fakeLogger, _mockFeatureFlags.Object);

        // Assert
        client.DefaultRequestHeaders.Contains("x-functions-key").Should().BeFalse();
        _fakeLogger.LatestRecord.Level.Should().Be(LogLevel.Warning);
        _fakeLogger.LatestRecord.Message.Should().Be("No FunctionKey provided - requests will be unauthenticated");
    }
}
