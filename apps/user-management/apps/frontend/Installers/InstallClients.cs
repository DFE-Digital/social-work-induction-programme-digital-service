using System.Diagnostics.CodeAnalysis;
using Dfe.Sww.Ecf.Frontend.HttpClients.Authentication;
using Dfe.Sww.Ecf.Frontend.HttpClients.Models;
using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland;
using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Options;
using Microsoft.Extensions.Options;

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

        services
            .AddHttpClient<
                SocialWorkEnglandClientOptions,
                ISocialWorkEnglandClient,
                SocialWorkEnglandClient
            >()
            .AddHttpMessageHandler<
                OAuthAuthenticationDelegatingHandler<SocialWorkEnglandClientOptions>
            >();
    }

    private static IHttpClientBuilder AddHttpClient<TOptions, TInterface, TConcrete>(
        this IServiceCollection services
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
}
