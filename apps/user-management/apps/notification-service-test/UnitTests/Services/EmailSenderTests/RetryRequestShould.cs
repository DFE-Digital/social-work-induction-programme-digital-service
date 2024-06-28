using DfeSwwEcf.NotificationService.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Notify.Exceptions;

namespace DfeSwwEcf.NotificationService.Tests.UnitTests.Services.EmailSenderTests;

public class RetryRequestShould : EmailSenderTestsTestBase
{
    [Theory]
    [MemberData(nameof(ExceptionTestData))]
    public void WhenPassedErrorToRetry_ShouldRethrowException(string errorMessage)
    {
        // Arrange
        var expectedException = new NotifyClientException(errorMessage);

        // Act
        var actualException = Assert.Throws<NotifyClientException>(
            () => Sut.RetryRequest(expectedException)
        );

        // Assert
        actualException.Should().BeEquivalentTo(expectedException);

        var expectedLog = $"Retrying request - {errorMessage}";
        MockLogger.Verify(
            x =>
                x.Log(
                    LogLevel.Information,
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
            new object[] { GovNotifyExceptionConstants.RATE_LIMIT_ERROR },
            new object[] { GovNotifyExceptionConstants.EXCEPTION }
        };
}
