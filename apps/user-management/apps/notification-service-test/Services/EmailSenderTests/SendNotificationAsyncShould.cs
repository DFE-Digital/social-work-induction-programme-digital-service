using System.Net;
using DfeSwwEcf.NotificationService.Models;
using DfeSwwEcf.NotificationService.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Notify.Exceptions;
using Notify.Interfaces;

namespace DfeSwwEcf.NotificationService.UnitTests.Services.EmailSenderTests;

public class RunAsyncShould
{
    private readonly Mock<IAsyncNotificationClient> _mockGovNotifyClient;
    private readonly Mock<ILogger<EmailNotificationCommand>> _mockLogger;
    private readonly EmailNotificationCommand _sut;

    public RunAsyncShould()
    {
        _mockGovNotifyClient = new();
        _mockLogger = new();
        _sut = new(_mockGovNotifyClient.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task WhenCalled_SendsEmailNotificationAsync()
    {
        // Arrange
        var notificationRequest = new NotificationRequest
        {
            EmailAddress = "test@test.com",
            TemplateId = Guid.NewGuid(),
            Personalisation = new()
            {
                { "first_name", "Amala" }
            },
            Reference = "string",
            EmailReplyToId = Guid.NewGuid()
        };

        var expectedPersonalisation = new Dictionary<string, dynamic>()
        {
            { "first_name", "Amala" }
        };

        var expectedResponse = new NotificationResponse
        {
            StatusCode = HttpStatusCode.OK
        };

        // Act
        var response = await _sut.SendNotificationAsync(notificationRequest);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<NotificationResponse>();
        response.Should().BeEquivalentTo(expectedResponse);

        _mockGovNotifyClient
            .Verify(x => x.SendEmailAsync(
                    notificationRequest.EmailAddress,
                    notificationRequest.TemplateId.ToString(),
                    expectedPersonalisation,
                    notificationRequest.Reference,
                    notificationRequest.EmailReplyToId.ToString())
                , Times.Once);
        _mockGovNotifyClient.VerifyNoOtherCalls();
    }

    [Theory]
    [MemberData(nameof(ExceptionTestData))]
    public async Task WhenClientThrowsException_ReturnRelevantStatusCode(HttpStatusCode expectedStatusCode,
        string errorMessage, int expectedClientCalls)
    {
        // Arrange
        var notificationRequest = new NotificationRequest
        {
            EmailAddress = "test@test.com",
            TemplateId = Guid.NewGuid(),
            Personalisation = new()
            {
                { "first_name", "Amala" }
            },
            Reference = "string",
            EmailReplyToId = Guid.NewGuid()
        };

        var expectedPersonalisation = new Dictionary<string, dynamic>
        {
            { "first_name", "Amala" }
        };

        var expectedResponse = new NotificationResponse
        {
            StatusCode = expectedStatusCode
        };

        var expectedException = new NotifyClientException(errorMessage);

        _mockGovNotifyClient.Setup(x => x.SendEmailAsync(
            notificationRequest.EmailAddress,
            notificationRequest.TemplateId.ToString(),
            expectedPersonalisation,
            notificationRequest.Reference,
            notificationRequest.EmailReplyToId.ToString()
        )).Throws(expectedException);

        // Act
        var response = await _sut.SendNotificationAsync(notificationRequest);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<NotificationResponse>();
        response.Should().BeEquivalentTo(expectedResponse);

        _mockGovNotifyClient
            .Verify(x => x.SendEmailAsync(
                    notificationRequest.EmailAddress,
                    notificationRequest.TemplateId.ToString(),
                    expectedPersonalisation,
                    notificationRequest.Reference,
                    notificationRequest.EmailReplyToId.ToString())
                , Times.Exactly(expectedClientCalls));
        _mockGovNotifyClient.VerifyNoOtherCalls();

        var expectedLog = $"{errorMessage} GovNotify error mapped to {expectedStatusCode}";
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) =>
                    string.Equals(expectedLog, o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
        _mockLogger.VerifyNoOtherCalls();
    }

    public static IEnumerable<object[]> ExceptionTestData =>
        new List<object[]>
        {
            new object[] { HttpStatusCode.TooManyRequests, GovNotifyExceptionConstants.RATE_LIMIT_ERROR, 6 },
            new object[] { HttpStatusCode.InternalServerError, GovNotifyExceptionConstants.EXCEPTION, 6 },
            new object[] { HttpStatusCode.BadRequest, GovNotifyExceptionConstants.BAD_REQUEST_ERROR, 1 },
            new object[] { HttpStatusCode.InternalServerError, GovNotifyExceptionConstants.AUTH_ERROR, 1 },
            new object[] { HttpStatusCode.TooManyRequests, GovNotifyExceptionConstants.TOO_MANY_REQUESTS_ERROR, 1 },
            new object[] { HttpStatusCode.InternalServerError, "UNKNOWN", 1 },
        };
}
