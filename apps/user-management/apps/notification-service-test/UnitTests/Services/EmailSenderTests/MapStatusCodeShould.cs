using System.Net;
using DfeSwwEcf.NotificationService.Models;
using DfeSwwEcf.NotificationService.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Notify.Exceptions;

namespace DfeSwwEcf.NotificationService.Tests.UnitTests.Services.EmailSenderTests;

public class MapStatusCodeShould : EmailSenderTestsTestBase
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

        var expectedResponse = new NotificationResponse { StatusCode = expectedStatusCode };

        // Act
        var response = Sut.MapStatusCode(exception);

        // Assert
        response.Should().BeEquivalentTo(expectedResponse);

        var expectedLog = $"{errorMessage} GovNotify error mapped to {expectedStatusCode}";
        MockLogger.Verify(
            x =>
                x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>(
                        (o, t) =>
                            string.Equals(
                                expectedLog,
                                o.ToString(),
                                StringComparison.InvariantCultureIgnoreCase
                            )
                    ),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
            Times.Once
        );

        VerifyAllNoOtherCall();
    }

    public static IEnumerable<object[]> ExceptionTestData =>
        new List<object[]>
        {
            new object[]
            {
                GovNotifyExceptionConstants.BAD_REQUEST_ERROR,
                HttpStatusCode.BadRequest
            },
            new object[]
            {
                GovNotifyExceptionConstants.AUTH_ERROR,
                HttpStatusCode.InternalServerError
            },
            new object[]
            {
                GovNotifyExceptionConstants.TOO_MANY_REQUESTS_ERROR,
                HttpStatusCode.TooManyRequests
            },
            new object[] { "UNKNOWN/ANYTHING ELSE", HttpStatusCode.InternalServerError }
        };
}
