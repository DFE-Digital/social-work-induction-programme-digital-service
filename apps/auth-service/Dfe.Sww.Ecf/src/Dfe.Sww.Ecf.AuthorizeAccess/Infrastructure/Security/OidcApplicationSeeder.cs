using Dfe.Sww.Ecf.AuthorizeAccess.Infrastructure.Security.Configuration;
using Dfe.Sww.Ecf.Core.DataStore.Postgres;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Infrastructure.Security;

public class OidcApplicationSeeder(IServiceProvider serviceProvider, IOptions<OidcOptions> oidcConfiguration)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await SeedOidcApplicationsAsync();
    }


    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task SeedOidcApplicationsAsync()
    {
        using var scope = serviceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<EcfDbContext>();
        await context.Database.EnsureCreatedAsync();

        var oidcApplicationConfigurations = oidcConfiguration.Value.Applications;
        if (oidcApplicationConfigurations is null)
        {
            throw new InvalidOperationException("Unable to parse configuration for OIDC applications.");
        }

        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        foreach (var oidcApplicationConfiguration in oidcApplicationConfigurations)
        {
            await SeedOidcApplicationAsync(manager, oidcApplicationConfiguration);
        }
    }

    private static async Task SeedOidcApplicationAsync(IOpenIddictApplicationManager manager,
        OidcApplicationConfiguration oidcApplicationConfiguration)
    {
        ArgumentNullException.ThrowIfNull(oidcApplicationConfiguration.ClientId);

        var oidcApplicationDescriptor = new OpenIddictApplicationDescriptor
        {
            ClientId = oidcApplicationConfiguration.ClientId,
            ClientSecret = oidcApplicationConfiguration.ClientSecret,
            ClientType = oidcApplicationConfiguration.ClientType,
            DisplayName = oidcApplicationConfiguration.DisplayName
        };

        oidcApplicationDescriptor.RedirectUris.AddRange(oidcApplicationConfiguration.RedirectUris);
        oidcApplicationDescriptor.PostLogoutRedirectUris.AddRange(oidcApplicationConfiguration.PostLogoutRedirectUris);

        oidcApplicationDescriptor.Permissions.AddRange(
            oidcApplicationConfiguration.AllowedEndpoints.Select(endpoint => "ept:" + endpoint));
        oidcApplicationDescriptor.Permissions.AddRange(
            oidcApplicationConfiguration.AllowedGrantTypes.Select(grantType => "gt:" + grantType));
        oidcApplicationDescriptor.Permissions.AddRange(
            oidcApplicationConfiguration.AllowedResponseTypes.Select(responseType => "rst:" + responseType));
        oidcApplicationDescriptor.Permissions.AddRange(
            oidcApplicationConfiguration.AllowedScopes.Select(scope => "scp:" + scope));
        if (oidcApplicationConfiguration.RequirePkce == true)
        {
            oidcApplicationDescriptor.Requirements.Add(
                OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange);
        }

        var oidcApplicationClient = await manager.FindByClientIdAsync(oidcApplicationDescriptor.ClientId);
        if (oidcApplicationClient is null)
        {
            await manager.CreateAsync(oidcApplicationDescriptor);
        }
        else
        {
            await manager.UpdateAsync(oidcApplicationClient, oidcApplicationDescriptor);
        }
    }
}
