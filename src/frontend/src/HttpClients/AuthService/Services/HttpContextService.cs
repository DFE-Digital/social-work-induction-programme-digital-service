using System.Security.Claims;
using SocialWorkInductionProgramme.Frontend.HttpClients.AuthService.Interfaces;

namespace SocialWorkInductionProgramme.Frontend.HttpClients.AuthService.Services;

public class HttpContextService(IHttpContextAccessor httpContextAccessor) : IHttpContextService
{
    public string GetOrganisationId()
    {
        return httpContextAccessor.HttpContext?.User.FindFirstValue("organisation_id") ??
               throw new NullReferenceException();
    }
}
