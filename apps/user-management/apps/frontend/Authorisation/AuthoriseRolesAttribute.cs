using Microsoft.AspNetCore.Authorization;

namespace Dfe.Sww.Ecf.Frontend.Authorisation;

public class AuthorizeRolesAttribute : AuthorizeAttribute
{
    public AuthorizeRolesAttribute(params RoleType[] roles)
        : base()
    {
        Roles = string.Join(",", roles);
    }
}
