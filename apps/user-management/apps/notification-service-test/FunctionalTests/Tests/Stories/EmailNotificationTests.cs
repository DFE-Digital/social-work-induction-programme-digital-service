using System.Net;
using DfeSwwEcf.NotificationService.Tests.FunctionalTests.Builders.TestData;
using DfeSwwEcf.NotificationService.Tests.FunctionalTests.Tests.Steps;
using FluentValidation.Results;
using TestStack.BDDfy;
using TestStack.BDDfy.Xunit;

namespace DfeSwwEcf.NotificationService.Tests.FunctionalTests.Tests.Stories;

// Define the story/feature being tested
[Story(
    AsA = "user of the notification service api",
    IWant = "to be able to send notifications with specific templates",
    SoThat = "I can ensure notifications are sent correctly"
)]
public class EmailNotificationTests
{
    private readonly EmailNotificationSteps _steps;

    public EmailNotificationTests()
    {
        _steps = new EmailNotificationSteps();
    }

    // Positive Test-Happy Path
    [BddfyTheory]
    [InlineData(
        "test@test.com",
        "2d6ad686-b5e7-466c-a810-60709b1b091c",
        "ABC123",
        HttpStatusCode.OK
    )]
    public async Task SendNotificationWithValidData(
        string emailAddress,
        string templateId,
        string personalText,
        HttpStatusCode expectedStatusCode
    )
    {
        _steps.GivenIHaveANotificationRequest(emailAddress, templateId, personalText);
        await _steps.WhenISendARequestAsync();
        _steps.ThenStatusCodeIsReturned(expectedStatusCode);
    }

    // Negative Test-UnHappy Path
    [BddfyTheory]
    [ClassData(typeof(InvalidNotificationTestData))]
    public async Task SendNotificationWithInValidData(
        string emailAddress,
        string templateId,
        string personalText,
        HttpStatusCode expectedStatusCode,
        List<ValidationFailure> expectedResponseBody
    )
    {
        _steps.GivenIHaveANotificationRequest(emailAddress, templateId, personalText);
        await _steps.WhenISendARequestAsync();
        _steps.ThenStatusCodeIsReturned(expectedStatusCode);
        await _steps.ThenResponseBodyIsReturnedAsync<List<ValidationFailure>>(expectedResponseBody);
    }

    // Negative Test-UnHappy Path 'Gov Notify Errors'
    [BddfyTheory]
    [ClassData(typeof(GovNotifyExceptionsTestData))]
    public async Task SendNotificationWhenGovNotifyThrowsException(
        string emailAddress,
        string templateId,
        string personalText,
        HttpStatusCode expectedStatusCode
    )
    {
        _steps.GivenIHaveANotificationRequest(emailAddress, templateId, personalText);
        switch (expectedStatusCode)
        {
            case HttpStatusCode.InternalServerError:
                await _steps.WhenGovNotifyThrowsInternalServerError();
                break;
            case HttpStatusCode.Unauthorized:
                await _steps.WhenGovNotifyFailsToAuthenticate();
                break;
            case HttpStatusCode.TooManyRequests:
                await _steps.WhenGovNotifyExceedsRateLimit();
                break;
            case HttpStatusCode.BadRequest:
                await _steps.WhenISendARequestAsync();
                break;
            default:
                throw new ArgumentOutOfRangeException(
                    nameof(expectedStatusCode),
                    expectedStatusCode,
                    null
                );
        }

        _steps.ThenStatusCodeIsReturned(expectedStatusCode);
    }

    // Negative Test-UnHappy Path 'Invalid JSON'
    [BddfyTheory]
    [ClassData(typeof(JsonExceptionsTestData))]
    public async Task SendNotificationWithInvalidJson(
        string emailAddress,
        string templateId,
        string personalText,
        HttpStatusCode expectedStatusCode,
        string expectedErrorMessage
    )
    {
        _steps.GivenIHaveANotificationRequest(emailAddress, templateId, personalText);
        await _steps.WhenISendARequestAsync();
        _steps.ThenStatusCodeIsReturned(expectedStatusCode);
        await _steps.ThenResponseStringIsReturnedAsync(expectedErrorMessage);
    }
}
