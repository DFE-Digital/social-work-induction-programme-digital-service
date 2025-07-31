using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Options;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RichardSzalay.MockHttp;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.HttpClient.NotificationServiceClientTests;

public class NotificationServiceClientTests
{
    private readonly Mock<ILogger<NotificationServiceClient>> _mockLogger = new();
    private readonly Mock<IOptions<NotificationClientOptions>> _mockOptions = new();

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
        _ = new NotificationServiceClient(client, _mockOptions.Object, _mockLogger.Object);

        // Assert
        client.DefaultRequestHeaders.Contains("x-functions-key").Should().BeTrue();
        client.DefaultRequestHeaders.GetValues("x-functions-key").First().Should().Be(functionKey);
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Adding x-functions-key header")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
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
        _ = new NotificationServiceClient(client, _mockOptions.Object, _mockLogger.Object);

        // Assert
        client.DefaultRequestHeaders.Contains("x-functions-key").Should().BeFalse();
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("No FunctionKey provided")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }
}
