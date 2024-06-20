using System.Net;
using DfeSwwEcf.NotificationService.Models;
using DfeSwwEcf.NotificationService.Services;
using DfeSwwEcf.NotificationService.UnitTests.Helpers;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Notify.Exceptions;
using Notify.Interfaces;

namespace DfeSwwEcf.NotificationService.UnitTests.Services.EmailSenderTests;

public class RunAsyncShould : EmailSenderTestsTestBase
{
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
        var response = await Sut.SendNotificationAsync(notificationRequest);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<NotificationResponse>();
        response.Should().BeEquivalentTo(expectedResponse);

        MockGovNotifyClient
            .Verify(x => x.SendEmailAsync(
                    notificationRequest.EmailAddress,
                    notificationRequest.TemplateId.ToString(),
                    expectedPersonalisation,
                    notificationRequest.Reference,
                    notificationRequest.EmailReplyToId.ToString())
                , Times.Once);

        VerifyAllNoOtherCall();
    }

    [Theory]
    [MemberData(nameof(ExceptionTestData))]
    public async Task WhenClientThrowsException_ReturnRelevantStatusCode(HttpStatusCode expectedStatusCode, string errorMessage, int expectedClientCalls)
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

        MockGovNotifyClient.Setup(x => x.SendEmailAsync(
            notificationRequest.EmailAddress,
            notificationRequest.TemplateId.ToString(),
            expectedPersonalisation,
            notificationRequest.Reference,
            notificationRequest.EmailReplyToId.ToString()
        )).Throws(expectedException);

        // Act
        var response = await Sut.SendNotificationAsync(notificationRequest);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<NotificationResponse>();
        response.Should().BeEquivalentTo(expectedResponse);

        MockGovNotifyClient
            .Verify(x => x.SendEmailAsync(
                    notificationRequest.EmailAddress,
                    notificationRequest.TemplateId.ToString(),
                    expectedPersonalisation,
                    notificationRequest.Reference,
                    notificationRequest.EmailReplyToId.ToString())
                , Times.Exactly(expectedClientCalls));
    }

    public static IEnumerable<object[]> ExceptionTestData =>
        new List<object[]>
        {
            new object[] { HttpStatusCode.TooManyRequests, GovNotifyExceptionConstants.RATE_LIMIT_ERROR, 6 },
            new object[] { HttpStatusCode.InternalServerError, GovNotifyExceptionConstants.EXCEPTION, 6 },
            new object[] { HttpStatusCode.BadRequest, GovNotifyExceptionConstants.BAD_REQUEST_ERROR, 1 },
            new object[] { HttpStatusCode.InternalServerError, GovNotifyExceptionConstants.AUTH_ERROR, 1 },
            new object[] { HttpStatusCode.TooManyRequests, GovNotifyExceptionConstants.TOO_MANY_REQUESTS_ERROR, 1 },
            new object[] { HttpStatusCode.InternalServerError, "UNKNOWN", 1 }
        };
}
