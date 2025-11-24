using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using Dfe.Sww.Ecf.Frontend.HttpClients.Authentication;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Options;
using Dfe.Sww.Ecf.Frontend.HttpClients.Models;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Options;
using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland;
using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Options;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;

namespace Dfe.Sww.Ecf.Frontend.Installers;

/// <summary>
/// Install Client Dependencies
/// </summary>
[ExcludeFromCodeCoverage]
public static class InstallClients
{
    /// <summary>
    /// Add Client Dependencies
    /// </summary>
    /// <param name="services"></param>
    public static void AddClients(this IServiceCollection services)
    {
        services.AddTransient(typeof(OAuthAuthenticationDelegatingHandler<>));
        services.AddTransient(typeof(OidcAuthenticationDelegatingHandler));

        // Social Work England Client
        services
            .AddHttpClient<
                SocialWorkEnglandClientOptions,
                ISocialWorkEnglandClient,
                SocialWorkEnglandClient
            >(true)
            .AddHttpMessageHandler<
                OAuthAuthenticationDelegatingHandler<SocialWorkEnglandClientOptions>
            >();

        services.AddResiliencePipeline<string, HttpResponseMessage>(
            nameof(SocialWorkEnglandClient),
            x =>
            {
                x.AddRetry(JitteredExponentialRetries([
                    HttpStatusCode.RequestTimeout,
                    HttpStatusCode.TooManyRequests,
                    HttpStatusCode.InternalServerError,
                    HttpStatusCode.ServiceUnavailable,
                    HttpStatusCode.GatewayTimeout
                ])).Build();
            }
        );

        // Notification Service Client
        services.AddHttpClient<
            NotificationClientOptions,
            INotificationServiceClient,
            NotificationServiceClient
        >();

        // Auth Service Client
        services
            .AddHttpClient<AuthClientOptions, IAuthServiceClient, AuthServiceClient>()
            .AddHttpMessageHandler<OidcAuthenticationDelegatingHandler>()
            .AddPolicyHandler((sp, request) =>
            {
                var logger = sp.GetRequiredService<ILogger<AuthServiceClient>>();

                return CreateStatusCodeRetryPolicy(
                    nameof(AuthServiceClient),
                    logger,
                    3,
                    HttpStatusCode.BadGateway,
                    HttpStatusCode.ServiceUnavailable,
                    HttpStatusCode.GatewayTimeout);
            });
    }

    private static IHttpClientBuilder AddHttpClient<TOptions, TInterface, TConcrete>(
        this IServiceCollection services,
        bool isSingleton = false
    )
        where TOptions : HttpClientOptions
        where TInterface : class
        where TConcrete : class, TInterface
    {
        var optionsName = typeof(TOptions).Name;
        services
            .AddOptions<TOptions>()
            .Configure<IConfiguration>(
                (options, configuration) => configuration.GetSection(optionsName).Bind(options)
            );

        if (isSingleton)
            services.AddSingleton<TInterface, TConcrete>();
        else
            services.AddTransient<TInterface, TConcrete>();

        var httpClientBuilder = services.AddHttpClient<TInterface, TConcrete>(
            (serviceProvider, client) =>
            {
                var service = serviceProvider.GetService<IOptions<TOptions>>();
                client.BaseAddress = new Uri(
                    service?.Value.BaseUrl ?? throw new InvalidOperationException()
                );
            }
        );

        return httpClientBuilder;
    }

    private static RetryStrategyOptions<HttpResponseMessage> JitteredExponentialRetries(ImmutableArray<HttpStatusCode> httpStatusCodes)
    {
        return new RetryStrategyOptions<HttpResponseMessage>
        {
            MaxRetryAttempts = 5,
            UseJitter = true,
            ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                .Handle<HttpRequestException>()
                .HandleResult(response => httpStatusCodes.Contains(response.StatusCode)),
            BackoffType = DelayBackoffType.Exponential
        };
    }

    private static IAsyncPolicy<HttpResponseMessage> CreateStatusCodeRetryPolicy(
        string clientName,
        ILogger logger,
        int retryCount,
        params HttpStatusCode[] statusCodes)
    {
        var statusSet = statusCodes.ToHashSet();

        return Policy<HttpResponseMessage>
            .HandleResult(r => r is not null && statusSet.Contains(r.StatusCode))
            .WaitAndRetryAsync(
                retryCount,
                attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                (outcome, timespan, attempt, _) =>
                {
                    var status = outcome.Result?.StatusCode;
                    var uri = outcome.Result?.RequestMessage?.RequestUri;

                    logger.LogWarning(
                        "[{ClientName}] Retry {Attempt} after status {StatusCode} ({StatusName}) for {RequestUri}. Delay {Delay}.",
                        clientName,
                        attempt,
                        (int?)status,
                        status,
                        uri,
                        timespan);
                });
    }
}
