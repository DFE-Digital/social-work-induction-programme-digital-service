using System.Net;
using Newtonsoft.Json;

namespace DfeSwwEcf.NotificationService.Tests.FunctionalTests.Builders.Http;

public class HttpRequestFactory
{
    public static async Task<HttpResponseMessage> GetAsync(string baseUrl, string path)
    {
        return await new HttpRequestBuilder()
            .AddMethod(HttpMethod.Get)
            .AddRequestUri(baseUrl, path)
            .SendAsync();
    }

    public static async Task<HttpResponseMessage> PostAsync<TContent>(
        string baseUrl,
        string path,
        TContent body
    )
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
            throw new NullReferenceException();

        var responseContent = await response.Content.ReadAsStringAsync();

        var deserializedResponse = JsonConvert.DeserializeObject<T>(responseContent);
        if (deserializedResponse == null)
            throw new NullReferenceException();

        return deserializedResponse;
    }

    public static async Task<string> ReadStringResponse(HttpResponseMessage response)
    {
        if (response == null)
            throw new NullReferenceException();

        var responseContent = await response.Content.ReadAsStringAsync();

        return responseContent;
    }

    public static async Task<HttpResponseMessage> SimulateInternalServerErrorAsync()
    {
        // Simulate an internal server error response
        var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent(
                JsonConvert.SerializeObject(
                    new
                    {
                        error = "InternalServerError",
                        message = "An internal server error occurred."
                    }
                )
            )
        };
        return await Task.FromResult(responseMessage);
    }

    public static async Task<HttpResponseMessage> SimulateAuthenticationFailureAsync()
    {
        // Simulate authentication failure response
        var responseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized)
        {
            Content = new StringContent(
                JsonConvert.SerializeObject(
                    new { error = "Unauthorized", message = "Authentication failed." }
                )
            )
        };
        return await Task.FromResult(responseMessage);
    }

    public static async Task<HttpResponseMessage> SimulateTooManyRequestsAsync()
    {
        var responseMessage = new HttpResponseMessage(HttpStatusCode.TooManyRequests)
        {
            Content = new StringContent(
                JsonConvert.SerializeObject(
                    new
                    {
                        error = "TooManyRequests",
                        message = "You have exceeded the limit of requests."
                    }
                )
            )
        };
        return await Task.FromResult(responseMessage);
    }
}
