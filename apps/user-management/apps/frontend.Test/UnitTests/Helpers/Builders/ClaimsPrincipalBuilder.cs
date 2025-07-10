using System.Security.Claims;
using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Models;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Builders;

public class ClaimsPrincipalBuilder
{
    private const string AuthenticationType = "TestAuthType";
    private readonly List<Claim> _claims = [];

    public ClaimsPrincipalBuilder WithRole(RoleType role)
    {
        _claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
        return this;
    }

    public ClaimsPrincipalBuilder WithRoles(IEnumerable<RoleType> roles)
    {
        foreach (var role in roles) WithRole(role);
        return this;
    }

    public ClaimsPrincipalBuilder WithEmail(string email)
    {
        _claims.Add(new Claim(ClaimTypes.Email, email));
        return this;
    }

    public ClaimsPrincipalBuilder WithName(string name)
    {
        _claims.Add(new Claim(ClaimTypes.Name, name));
        return this;
    }

    public ClaimsPrincipalBuilder WithClaim(string type, string value)
    {
        _claims.Add(new Claim(type, value));
        return this;
    }

    public ClaimsPrincipalBuilder WithClaim(Claim claim)
    {
        _claims.Add(claim);
        return this;
    }

    public ClaimsPrincipal Build()
    {
        var identity = new ClaimsIdentity(_claims, AuthenticationType);
        return new ClaimsPrincipal(identity);
    }
}
