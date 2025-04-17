using System.Net.Http.Headers;
using System.Text.Json;
using Dfe.Sww.Ecf.Frontend.HttpClients.Models;
using Microsoft.Extensions.Options;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.Authentication;

public class OAuthAuthenticationDelegatingHandler<TOptions> : DelegatingHandler
    where TOptions : HttpClientOptions
{
    private const string TokenEndpoint = "connect/token";

    private readonly ClientCredentials _clientCredentials;
    private readonly HttpClient _client;
    private static AccessToken? _accessToken;
    private static readonly SemaphoreSlim AccessTokenSemaphore = new(1, 1);

    public OAuthAuthenticationDelegatingHandler(IOptions<TOptions> options, HttpClient client)
    {
        _clientCredentials = options.Value.ClientCredentials;
        _client = client;

        if (_client.BaseAddress == null)
        {
            _client.BaseAddress = new Uri(_clientCredentials.AccessTokenUrl);
        }

        if (_client.BaseAddress?.AbsoluteUri.EndsWith('/') == false)
        {
            _client.BaseAddress = new Uri(_client.BaseAddress.AbsoluteUri + "/");
        }
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        var token = await FetchTokenAsync();

        request.Headers.Authorization = new AuthenticationHeaderValue(token.Scheme, token.Token);

        return await base.SendAsync(request, cancellationToken);
    }

    private async Task<AccessToken> FetchTokenAsync()
    {
        try
        {
            await AccessTokenSemaphore.WaitAsync();

            if (_accessToken is { Expired: false })
            {
                return _accessToken;
            }

            var request = new HttpRequestMessage(HttpMethod.Post, TokenEndpoint)
            {
                Content = new FormUrlEncodedContent(
                    new KeyValuePair<string?, string?>[]
                    {
                        new("client_id", _clientCredentials.ClientId),
                        new("client_secret", _clientCredentials.ClientSecret),
                        new("grant_type", "client_credentials")
                    }
                )
            };

            using var response = await _client.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead
            );

            response.EnsureSuccessStatusCode();

            await using var responseContentStream = await response.Content.ReadAsStreamAsync();

            var accessToken = await JsonSerializer.DeserializeAsync<AccessToken>(
                responseContentStream
            );

            _accessToken = accessToken ?? throw new Exception("Failed to deserialize access token");

            return accessToken;
        }
        finally
        {
            AccessTokenSemaphore.Release(1);
        }
    }
}
