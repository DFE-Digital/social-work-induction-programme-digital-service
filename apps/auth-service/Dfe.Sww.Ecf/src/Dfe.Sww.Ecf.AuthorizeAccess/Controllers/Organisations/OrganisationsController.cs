using Dfe.Sww.Ecf.Core.Models.Pagination;
using Dfe.Sww.Ecf.Core.Services.Organisations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Controllers.Organisations;

[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/[controller]")]
public class OrganisationsController(
    IOrganisationService organisationService
) : Controller
{
    [HttpGet]
    [ActionName(nameof(GetAllAsync))]
    public async Task<IActionResult> GetAllAsync([FromQuery] PaginationRequest request)
    {
        var organisations = await organisationService.GetAllAsync(request);
        if (!organisations.Records.Any())
        {
            return NoContent();
        }

        return Ok(organisations);
    }
}
