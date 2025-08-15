using System.Text.Json;
using Dfe.Sww.Ecf.Frontend.Helpers;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Operations;

public class BaseOperations
{
    protected static JsonSerializerOptions? SerializerOptions { get; } =
        new(JsonSerializerDefaults.Web) { Converters = { new BooleanConverter() } };

    protected static void HandleHttpResponse(HttpResponseMessage httpResponse, string? failureMessage = null)
    {
        if (!httpResponse.IsSuccessStatusCode)
        {
            var message = failureMessage ?? $"API request failed with status code {httpResponse.StatusCode}";
            throw new HttpRequestException(message, null, httpResponse.StatusCode);
        }
    }

}
