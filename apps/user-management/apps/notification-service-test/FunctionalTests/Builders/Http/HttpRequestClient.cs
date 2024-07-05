using System.Collections.Concurrent;

namespace DfeSwwEcf.NotificationService.Tests.FunctionalTests.Builders.Http
{
    public static class HttpRequestClient
    {
        private static ConcurrentDictionary<string, HttpClient> _httpClientList =
            new ConcurrentDictionary<string, HttpClient>();

        public static HttpClient GetHttpClientInstance(string baseUrl)
        {
            if (!_httpClientList.ContainsKey(baseUrl))
            {
                _httpClientList.TryAdd(baseUrl, new HttpClient());
            }

            return _httpClientList[baseUrl];
        }
    }
}
