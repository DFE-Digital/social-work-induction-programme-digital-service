using System.Net.Mime;
using Dfe.Sww.Ecf.Core.Services.Organisations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Controllers.LocalAuthorities;

[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/[controller]")]
public class LocalAuthorityController(ILocalAuthorityService localAuthorityService) : Controller
{
    [HttpGet("{localAuthorityCode:int}")]
    [ActionName(nameof(GetByCodeAsync))]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetByCodeAsync(int localAuthorityCode)
    {
        var localAuthority = await localAuthorityService.GetByCodeAsync(localAuthorityCode);
        if (localAuthority == null)
        {
            return NoContent();
        }

        return Ok(localAuthority);
    }
}
