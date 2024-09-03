using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Threading.RateLimiting;
using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland;
using Dfe.Sww.Ecf.Frontend.Services;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Polly;
using Polly.Retry;

namespace Dfe.Sww.Ecf.Frontend.Installers;

/// <summary>
/// Install Service Dependencies
/// </summary>
[ExcludeFromCodeCoverage]
public static class InstallServices
{
    /// <summary>
    /// Add Service Dependencies
    /// </summary>
    /// <param name="services"></param>
    public static void AddServices(this IServiceCollection services)
    {
        services.AddResiliencePipeline<string, HttpResponseMessage>(
            nameof(SocialWorkEnglandClient),
            x =>
            {
                x.AddRetry(JitteredExponentialRetries()).Build();
            }
        );

        services.AddTransient<ISocialWorkEnglandService, SocialWorkEnglandService>();
    }

    private static RetryStrategyOptions<HttpResponseMessage> JitteredExponentialRetries()
    {
        return new RetryStrategyOptions<HttpResponseMessage>
        {
            MaxRetryAttempts = 5,
            UseJitter = true,
            ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                .Handle<HttpRequestException>(ex => ex.InnerException is SocketException)
                .HandleResult(response => RetriableStatuses().Contains(response.StatusCode)),
            BackoffType = DelayBackoffType.Exponential
        };
    }

    private static ImmutableArray<HttpStatusCode> RetriableStatuses()
    {
        return
        [
            .. new[]
            {
                HttpStatusCode.RequestTimeout,
                HttpStatusCode.TooManyRequests,
                HttpStatusCode.InternalServerError,
                HttpStatusCode.ServiceUnavailable,
                HttpStatusCode.GatewayTimeout,
            }
        ];
    }
}
