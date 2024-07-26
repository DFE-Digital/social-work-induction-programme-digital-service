using Microsoft.Extensions.Logging;
using Moq;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;

public static class LoggerExtensions
{
    public static Mock<ILogger<T>> VerifyLog<T>(
        this Mock<ILogger<T>> logger,
        string expectedMessage,
        LogLevel expectedLogLevel = LogLevel.Debug,
        Times? times = null
    )
    {
        times ??= Times.Once();

        Func<object, Type, bool> state = (o, _) =>
            string.Equals(o.ToString(), expectedMessage, StringComparison.Ordinal);

        logger.Verify(
            x =>
                x.Log(
                    It.Is<LogLevel>(l => l == expectedLogLevel),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => state(v, t)),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!
                ),
            (Times)times
        );

        return logger;
    }
}
