using System.Text;
using Newtonsoft.Json;

namespace DfeSwwEcf.NotificationService.Tests.FunctionalTests.Builders.Http
{
    public class HttpRequestBuilder
    {
        private HttpMethod _method = HttpMethod.Get;
        private string? _path;
        private string? _baseUrl;
        private HttpContent? _content;

        public HttpRequestBuilder AddMethod(HttpMethod method)
        {
            _method = method;
            return this;
        }

        public HttpRequestBuilder AddRequestUri(string baseUrl, string requestUri)
        {
            _baseUrl = baseUrl;
            _path = requestUri;
            return this;
        }

        public HttpRequestBuilder AddContent<TContent>(TContent body)
        {
            var json = JsonConvert.SerializeObject(body);
            _content = new StringContent(json, Encoding.UTF8, "application/json");
            return this;
        }

        public async Task<HttpResponseMessage> SendAsync()
        {
            var request = new HttpRequestMessage
            {
                Method = _method,
                RequestUri = new Uri($"{_baseUrl}{_path}")
            };

            if (_content != null)
            {
                request.Content = _content;
            }

            if (string.IsNullOrWhiteSpace(_baseUrl))
            {
                throw new NullReferenceException();
            }

            var httpClient = HttpRequestClient.GetHttpClientInstance(_baseUrl);
            return await httpClient.SendAsync(request);
        }
    }
}
