using System.Net.Mime;
using Dfe.Sww.Ecf.Core.Services.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Controllers.AsyeSocialWorkers;

[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/[controller]")]
public class AsyeSocialWorkerController(
    IAsyeSocialWorkerService asyeSocialWorkerService
) : Controller
{
    [HttpGet("{socialWorkerId}")]
    [ActionName(nameof(Exists))]
    [Produces(MediaTypeNames.Application.Json)]
    public IActionResult Exists(string socialWorkerId)
    {
        var exists = asyeSocialWorkerService.Exists(socialWorkerId);

        return Ok(exists);
    }
}
