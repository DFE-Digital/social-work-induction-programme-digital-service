using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.Authentication;

public class OidcAuthenticationDelegatingHandler(IHttpContextAccessor httpContextAccessor)
    : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        var token = await FetchToken();

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }

    private async Task<string> FetchToken()
    {
        var accessToken = await (
            httpContextAccessor.HttpContext ?? throw new InvalidOperationException()
        ).GetTokenAsync("access_token");
        if (string.IsNullOrEmpty(accessToken))
        {
            throw new UnauthorizedAccessException("Access token is not available");
        }
        return accessToken;
    }
}
