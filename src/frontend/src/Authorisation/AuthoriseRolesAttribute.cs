using Microsoft.AspNetCore.Authorization;

namespace SocialWorkInductionProgramme.Frontend.Authorisation;

public class AuthorizeRolesAttribute : AuthorizeAttribute
{
    public AuthorizeRolesAttribute(params RoleType[] roles)
        : base()
    {
        Roles = string.Join(",", roles);
    }
}
