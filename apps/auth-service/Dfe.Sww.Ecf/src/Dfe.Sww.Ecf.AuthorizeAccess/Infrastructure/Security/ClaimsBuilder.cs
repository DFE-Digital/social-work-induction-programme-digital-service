using System.Security.Claims;
using Dfe.Sww.Ecf.Core.DataStore.Postgres;
using OpenIddict.Abstractions;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Infrastructure.Security;

public class ClaimsBuilder(ClaimsIdentity identity, OpenIddictRequest request)
{
    private readonly ClaimsIdentity _identity =
        identity ?? throw new ArgumentNullException(nameof(identity));
    private readonly OpenIddictRequest _request =
        request ?? throw new ArgumentNullException(nameof(request));

    /// <summary>
    /// Add a claim if the specified scope is present.
    /// </summary>
    public ClaimsBuilder AddIfScope(string scope, string claimType, Func<string?> valueProvider)
    {
        if (!_request.HasScope(scope))
        {
            return this;
        }

        var value = valueProvider();
        if (!string.IsNullOrEmpty(value))
        {
            _identity.SetClaim(claimType, value);
        }
        return this;
    }

    /// <summary>
    /// Add multiple role claims if the "roles" scope is present.
    /// </summary>
    public async Task<ClaimsBuilder> AddRoleClaimsIfScopeAsync(
        string scope,
        Guid personId,
        EcfDbContext dbContext
    )
    {
        if (!_request.HasScope(scope))
        {
            return this;
        }

        var roleClaims = await dbContext
            .PersonRoles.Where(pr => pr.PersonId == personId)
            .Select(pr => new Claim(
                System.Security.Claims.ClaimTypes.Role,
                pr.Role.RoleName.ToString()
            ))
            .ToListAsync();

        _identity.AddClaims(roleClaims);
        return this;
    }

    /// <summary>
    /// Add the organisation id claim if the "organisation" scope is present.
    /// </summary>
    public async Task<ClaimsBuilder> AddOrganisationIdClaimIfScopeAsync(
        string scope,
        Guid personId,
        EcfDbContext dbContext
    )
    {
        if (!_request.HasScope(scope))
        {
            return this;
        }

        var organisationIdClaim = await dbContext
            .PersonOrganisations.Where(po => po.PersonId == personId)
            .Where(po => po.EndDate == null || (po.StartDate >= po.EndDate))
            .Select(po => po.OrganisationId.ToString()
            )
            .FirstAsync();

        if (!string.IsNullOrEmpty(organisationIdClaim))
        {
            _identity.SetClaim(ClaimTypes.OrganisationId, organisationIdClaim);
        }

        return this;
    }
}
