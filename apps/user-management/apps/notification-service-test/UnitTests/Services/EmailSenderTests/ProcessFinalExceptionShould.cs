using System.Net;
using DfeSwwEcf.NotificationService.Models;
using DfeSwwEcf.NotificationService.Services;
using DfeSwwEcf.NotificationService.Tests.UnitTests.Helpers;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Notify.Exceptions;
using Polly;

namespace DfeSwwEcf.NotificationService.Tests.UnitTests.Services.EmailSenderTests;

public class ProcessFinalExceptionShould : EmailSenderTestsTestBase
{
    [Theory]
    [MemberData(nameof(ExceptionTestData))]
    public void WhenPassedException_ShouldMapExceptionToStatusCode(
        string errorMessage,
        HttpStatusCode expectedStatusCode
    )
    {
        // Arrange
        var exception = new NotifyClientException(errorMessage);
        var pollyResult = PolicyResult<NotificationResponse>.Failure(
            exception,
            ExceptionType.Unhandled,
            new Context()
        );

        var expectedResponse = new NotificationResponse { StatusCode = expectedStatusCode };

        // Act
        var response = Sut.ProcessFinalException(pollyResult);

        // Assert
        response.Should().BeEquivalentTo(expectedResponse);

        var expectedLog =
            $"Maximum retries reached - {errorMessage} GovNotify error mapped to {expectedStatusCode}";
        MockLogger.VerifyLog(expectedLog, LogLevel.Error);

        VerifyAllNoOtherCall();
    }

    public static IEnumerable<object[]> ExceptionTestData =>
        new List<object[]>
        {
            new object[]
            {
                GovNotifyExceptionConstants.EXCEPTION,
                HttpStatusCode.InternalServerError
            },
            new object[]
            {
                GovNotifyExceptionConstants.RATE_LIMIT_ERROR,
                HttpStatusCode.TooManyRequests
            },
            new object[] { "UNKNOWN/ANYTHING ELSE", HttpStatusCode.InternalServerError }
        };
}
