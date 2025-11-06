using System.Net.Mime;
using Dfe.Sww.Ecf.Core.Models.Pagination;
using Dfe.Sww.Ecf.Core.Services.Organisations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Controllers.Organisations;

[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/[controller]")]
public class OrganisationsController(IOrganisationService organisationService) : Controller
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

    [HttpGet("{id:guid}")]
    [ActionName(nameof(GetByIdAsync))]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var organisation = await organisationService.GetByIdAsync(id);
        if (organisation == null)
        {
            return NoContent();
        }

        return Ok(organisation);
    }

    [HttpPost("Create")]
    [ActionName(nameof(CreateAsync))]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreateOrganisationRequest createOrganisationRequest
    )
    {
        var createdOrganisation = await organisationService.CreateAsync(
            createOrganisationRequest.ToOrganisation()
        );

        return CreatedAtAction(
            nameof(GetByIdAsync),
            new { id = createdOrganisation.OrganisationId },
            createdOrganisation
        );
    }

    [HttpGet("{localAuthorityCode:int}")]
    [ActionName(nameof(GetByLocalAuthorityCodeAsync))]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetByLocalAuthorityCodeAsync(int localAuthorityCode)
    {
        var organisation = await organisationService.GetByLocalAuthorityCodeAsync(
            localAuthorityCode
        );
        if (organisation == null)
        {
            return NoContent();
        }

        return Ok(organisation);
    }
}
