using System.Collections.Concurrent;
using System.Text.Json;
using Dfe.Sww.Ecf.Frontend.Helpers;
using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Models;
using Polly;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Operations;

public class SocialWorkersOperations(
    SocialWorkEnglandClient socialWorkEnglandClient,
    ResiliencePipeline<HttpResponseMessage> pipeline
) : ISocialWorkersOperations
{
    private static JsonSerializerOptions? SerializerOptions { get; } =
        new(JsonSerializerDefaults.Web) { Converters = { new BooleanConverter() } };
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly ConcurrentQueue<TaskCompletionSource<SocialWorker?>> _queue = new();

    private readonly SocialWorkEnglandClient _socialWorkEnglandClient = socialWorkEnglandClient;
    private readonly ResiliencePipeline<HttpResponseMessage> _pipeline = pipeline;

    public async Task<SocialWorker?> GetByIdAsync(int id)
    {
        var tcs = new TaskCompletionSource<SocialWorker?>();
        _queue.Enqueue(tcs);

        await ProcessQueueAsync(id);

        return await tcs.Task;
    }

    private async Task ProcessQueueAsync(int id)
    {
        await _semaphore.WaitAsync();

        try
        {
            while (_queue.TryDequeue(out var tcs))
            {
                var route =
                    _socialWorkEnglandClient.Options.Routes.SocialWorker.GetById;

                var httpResponse = await MakeGetRequestWithRetryAsync(route + $"?swid={id}");

                if (httpResponse is null || !httpResponse.IsSuccessStatusCode)
                {
                    tcs.SetResult(null);
                    return;
                }

                var result = await httpResponse.Content.ReadAsStringAsync();

                // Invalid request is a 200 response
                if (result == "Invalid request")
                {
                    tcs.SetResult(null);
                    return;
                }

                var response = JsonSerializer.Deserialize<SocialWorker>(result, SerializerOptions);
                tcs.SetResult(response);
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task<HttpResponseMessage?> MakeGetRequestWithRetryAsync(string route)
    {
        try
        {
            return await _pipeline.ExecuteAsync(async ct =>
            {
                var httpResponse = await _socialWorkEnglandClient.HttpClient.GetAsync(route, ct);

                return httpResponse;
            });
        }
        catch (Exception e)
        {
            return null;
        }
    }
}
