using System.Security.Claims;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Services;

public class HttpContextService(IHttpContextAccessor httpContextAccessor) : IHttpContextService
{
    public string GetOrganisationId()
    {
        return httpContextAccessor.HttpContext?.User.FindFirstValue("organisation_id") ??
               throw new NullReferenceException();
    }
}
