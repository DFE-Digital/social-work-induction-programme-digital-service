using System.Security.Cryptography.X509Certificates;
using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Secrets;
using Dfe.Sww.Ecf.AuthorizeAccess.Infrastructure.Security.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class KeysController(
    CertificateClient certClient,
    SecretClient secretClient,
    IMemoryCache cache,
    OneLoginConfiguration oneLoginConfiguration
) : Controller
{
    [HttpGet("onelogin")]
    [ResponseCache(Duration = 3600)]
    public async Task<IActionResult> GetOneLoginKeys()
    {
        // This method provides an /api/keys/onelogin JWKS endpoint for use with OneLogin
        // It lists the public keys which are supported for signing JWTs
        if (cache.TryGetValue<JsonWebKeySet>("OneLoginJWKS", out var jwks))
        {
            return Ok(jwks);
        }

        var versions = certClient
            .GetPropertiesOfCertificateVersionsAsync(oneLoginConfiguration.CertificateName)
            .OrderByDescending(p => p.CreatedOn);

        var now = DateTimeOffset.UtcNow;
        var toInclude = new List<X509Certificate2>();

        // Always include the latest version of the public key
        var latestMeta = await versions.FirstAsync();
        toInclude.Add(await FetchCertAsync(latestMeta.Version));

        // Optionally include the previous if within overlap window
        var prevMeta = await versions.Skip(1).FirstOrDefaultAsync();
        if (prevMeta?.ExpiresOn?.AddHours(24) > now)
        {
            toInclude.Add(await FetchCertAsync(prevMeta.Version));
        }

        jwks = new JsonWebKeySet();

        foreach(var cert in toInclude)
        {
            var rsa = cert.GetRSAPublicKey() ?? throw new InvalidOperationException("Not RSA");
            var parameters = rsa.ExportParameters(false);

            var n = Base64UrlEncoder.Encode(parameters.Modulus);
            var e = Base64UrlEncoder.Encode(parameters.Exponent);

            var jwk = new JsonWebKey
            {
                Kid = cert.Thumbprint,
                Kty = JsonWebAlgorithmsKeyTypes.RSA,
                Use = JsonWebKeyUseNames.Sig,
                N = n,
                E = e
            };

            jwks.Keys.Add(jwk);
        }        

        cache.Set("OneLoginJWKS", jwks, TimeSpan.FromMinutes(60));

        Response.Headers.Add("kid", toInclude.First().Thumbprint);

        return Ok(jwks);
    }

    private async Task<X509Certificate2> FetchCertAsync(string version)
    {
        var secret = await secretClient
            .GetSecretAsync(oneLoginConfiguration.CertificateName, version);
        var raw = Convert.FromBase64String(secret.Value.Value);
        
        return new X509Certificate2(
            raw,
            (string?)null,
            X509KeyStorageFlags.EphemeralKeySet);
    }
}
