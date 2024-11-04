using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using Dfe.Sww.Ecf.Frontend.HttpClients.AccountsService;
using Dfe.Sww.Ecf.Frontend.HttpClients.AccountsService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.AccountsService.Options;
using Dfe.Sww.Ecf.Frontend.HttpClients.Authentication;
using Dfe.Sww.Ecf.Frontend.HttpClients.Models;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Options;
using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland;
using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Options;
using Microsoft.Extensions.Options;
using Polly;
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

        services.AddResiliencePipeline<string, HttpResponseMessage>(
            nameof(SocialWorkEnglandClient),
            x =>
            {
                x.AddRetry(JitteredExponentialRetries()).Build();
            }
        );

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

        // Notification Service Client
        services.AddHttpClient<
            NotificationClientOptions,
            INotificationServiceClient,
            NotificationServiceClient
        >();

        // Auth Service Client
        services
            .AddHttpClient<AuthClientOptions, IAuthServiceClient, AuthServiceClient>()
            .AddHttpMessageHandler<OidcAuthenticationDelegatingHandler>();
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
        {
            services.AddSingleton<TInterface, TConcrete>();
        }
        else
        {
            services.AddTransient<TInterface, TConcrete>();
        }

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
