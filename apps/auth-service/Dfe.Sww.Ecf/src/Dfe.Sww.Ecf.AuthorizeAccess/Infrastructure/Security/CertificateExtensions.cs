using System.Security.Cryptography.X509Certificates;
using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Secrets;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Infrastructure.Security;

public static class KeyVaultCertificateExtensions
{
    public static async Task<X509Certificate2> GetX509CertificateAsync(
        this CertificateClient certClient,
        string certificateName,
        TokenCredential? credential = null)
    {
        KeyVaultCertificateWithPolicy certWithPolicy =
            await certClient.GetCertificateAsync(certificateName)
                .ConfigureAwait(false);

        var secretClient = new SecretClient(
            certClient.VaultUri,
            credential ?? new DefaultAzureCredential());

        var secretName = certWithPolicy.Name!;
        var secretVersion = certWithPolicy.Properties.Version!;

        KeyVaultSecret secret =
            await secretClient.GetSecretAsync(secretName, secretVersion)
                .ConfigureAwait(false);

        var pfxBytes = Convert.FromBase64String(secret.Value);

        return new X509Certificate2(
            pfxBytes,
            (string?)null,
            X509KeyStorageFlags.EphemeralKeySet
        );
    }
}
