using System.Net;
using DfeSwwEcf.NotificationService.Tests.FunctionalTests.Builders.Http;
using DfeSwwEcf.NotificationService.Tests.FunctionalTests.Configuration;
using DfeSwwEcf.NotificationService.Tests.FunctionalTests.Models;
using FluentAssertions;

namespace DfeSwwEcf.NotificationService.Tests.FunctionalTests.Tests.Steps;

public class EmailNotificationSteps
{
    private HttpResponseMessage? _response;
    private EmailNotificationRequest? _requestBody;
    private readonly string _baseUrl;

    public EmailNotificationSteps()
    {
        var config = ConfigAccessor.GetApplicationConfiguration();
        _baseUrl = config.BaseUrl;
    }

    public void GivenIHaveANotificationRequest(
        string emailAddress,
        string templateId,
        string personalText
    )
    {
        _requestBody = new EmailNotificationRequest()
        {
            EmailAddress = emailAddress,
            TemplateId = templateId,
            Personalisation = new Dictionary<string, string> { { "personal_text", personalText } }
        };
    }

    public async Task WhenISendARequestAsync()
    {
        if (_requestBody == null)
            return;

        _response = await HttpRequestFactory.PostAsync(_baseUrl, "Notification", _requestBody);
    }

    public async Task ThenResponseBodyIsReturnedAsync<T>(object expectedResponseBody)
    {
        if (_response == null)
            throw new NullReferenceException();

        var responseBody = await HttpRequestFactory.ReadResponseAsync<T>(_response);
        responseBody
            .Should()
            .BeEquivalentTo(
                expectedResponseBody,
                options =>
                    options
                        .IncludingAllDeclaredProperties()
                        .ExcludingMissingMembers()
                        .WithStrictOrdering()
            );
    }

    public async Task ThenResponseStringIsReturnedAsync(string expectedResponseBody)
    {
        if (_response == null)
            throw new NullReferenceException();

        var responseBody = await HttpRequestFactory.ReadStringResponse(_response);

        responseBody.Should().BeEquivalentTo(expectedResponseBody);
    }

    public async Task WhenGovNotifyThrowsInternalServerError()
    {
        // Simulate Gov Notify throwing an internal server error
        _response = await HttpRequestFactory.SimulateInternalServerErrorAsync();
    }

    public async Task WhenGovNotifyFailsToAuthenticate()
    {
        // Simulate Gov Notify failing to authenticate
        _response = await HttpRequestFactory.SimulateAuthenticationFailureAsync();
    }

    public async Task WhenGovNotifyExceedsRateLimit()
    {
        // Simulate Gov Notify rate limit exceeded due to too many requests
        _response = await HttpRequestFactory.SimulateTooManyRequestsAsync();
    }

    public void ThenStatusCodeIsReturned(HttpStatusCode expectedStatusCode)
    {
        if (_response == null)
            throw new NullReferenceException();

        _response.StatusCode.Should().Be(expectedStatusCode);
    }
}
