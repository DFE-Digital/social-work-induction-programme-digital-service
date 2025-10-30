using System.Security.Claims;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Services;

public class HttpContextService(IHttpContextAccessor httpContextAccessor) : IHttpContextService
{
    public Guid GetPersonId()
    {
        var personIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue("person_id");
        var isSuccessful = Guid.TryParse(personIdClaim, out var personId);

        return isSuccessful
            ? personId
            : throw new NullReferenceException();
    }

    public string GetOrganisationId()
    {
        return httpContextAccessor.HttpContext?.User.FindFirstValue("organisation_id") ??
               throw new NullReferenceException();
    }

    public bool GetIsEcswRegistered()
    {
        var successful = bool.TryParse(httpContextAccessor.HttpContext?.User.FindFirstValue("is_ecsw_registered"),
            out var isEcswRegistered);
        if (successful == false)
        {
            // Key isn't present, they don't need to register
            return true;
        }

        return isEcswRegistered;
    }

    public bool GetIsStaffFirstLogin()
    {
        var successful = bool.TryParse(httpContextAccessor.HttpContext?.User.FindFirstValue("is_staff_first_login"),
            out var isStaffFirstLogin);
        if (successful == false)
        {
            // Key isn't present, not first login for staff account
            return false;
        }

        return isStaffFirstLogin;
    }
}
