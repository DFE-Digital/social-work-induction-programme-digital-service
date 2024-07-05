using Newtonsoft.Json;

namespace DfeSwwEcf.NotificationService.Tests.FunctionalTests.Builders.Http
{
    public class HttpRequestFactory
    {
        public static async Task<HttpResponseMessage> GetAsync(string baseUrl, string path)
        {
            return await new HttpRequestBuilder()
                .AddMethod(HttpMethod.Get)
                .AddRequestUri(baseUrl, path)
                .SendAsync();
        }

        public static async Task<HttpResponseMessage> PostAsync<TContent>(string baseUrl, string path, TContent body)
        {
            return await new HttpRequestBuilder()
                .AddMethod(HttpMethod.Post)
                .AddRequestUri(baseUrl, path)
                .AddContent(body)
                .SendAsync();
        }

        public static async Task<T> ReadResponseAsync<T>(HttpResponseMessage response)
        {
            if (response == null)
            {
                throw new NullReferenceException();
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<T>(responseContent);
            if (deserializedResponse == null)
            {
                throw new NullReferenceException();
            }

            return deserializedResponse;
        }
    }
}
