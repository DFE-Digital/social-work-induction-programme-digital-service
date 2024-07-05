using System.Net;
using DfeSwwEcf.NotificationService.Tests.FunctionalTests.Builders.Http;
using DfeSwwEcf.NotificationService.Tests.FunctionalTests.Configuration;
using DfeSwwEcf.NotificationService.Tests.FunctionalTests.Models;
using FluentAssertions;

namespace DfeSwwEcf.NotificationService.Tests.FunctionalTests.Tests.Steps
{
    public class EmailNotificationSteps
    {
        private HttpResponseMessage? _response;
        private EmailNotificationRequest?  _requestBody;
        private readonly string _baseUrl;

        public EmailNotificationSteps()
        {
            var config = ConfigAccessor.GetApplicationConfiguration();
            _baseUrl = config.BaseUrl;
        }

        public void GivenIHaveANotificationRequest(string emailAddress, Guid templateId, string personalText)
        {
            _requestBody = new EmailNotificationRequest()
            {
                EmailAddress = emailAddress,
                TemplateId = templateId,
                Personalisation = new ()
                {
                    {"personal_text",personalText}
                }
            };
        }

        public async Task WhenISendARequestAsync()
        {
            if (_requestBody == null)
            {
                return;
            }
            _response= await HttpRequestFactory.PostAsync(_baseUrl, "Notification",_requestBody);
        }

        public async Task  ThenResponseBodyIsReturnedAsync <T>(object expectedResponseBody)
        {
            if (_response == null)
            {
                throw new NullReferenceException();
            }
            var responseBody = await HttpRequestFactory.ReadResponseAsync<T>(_response);
             responseBody.Should().BeEquivalentTo(expectedResponseBody);
        }

        public void ThenStatusCodeIsReturned(HttpStatusCode expectedStatusCode)
        {
            if (_response == null)
            {
                throw new NullReferenceException();
            }
            _response.StatusCode.Should().Be(expectedStatusCode);
        }
    }
}

