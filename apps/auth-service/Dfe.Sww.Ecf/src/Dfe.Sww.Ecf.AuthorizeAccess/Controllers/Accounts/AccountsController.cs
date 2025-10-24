using System.Net.Mime;
using Dfe.Sww.Ecf.Core.Models.Pagination;
using Dfe.Sww.Ecf.Core.Services.Accounts;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Controllers.Accounts;

[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/[controller]")]
public class AccountsController(
    IAccountsService accountsService,
    IOneLoginAccountLinkingService oneLoginAccountLinkingService,
    AppInfo appInfo
) : Controller
{
    [HttpGet]
    [ActionName(nameof(GetAllAsync))]
    public async Task<IActionResult> GetAllAsync([FromQuery] PaginationRequest request, [FromQuery] string organisationId)
    {
        if (!Guid.TryParse(organisationId, out Guid parsedOrganisationId))
        {
            return BadRequest("Invalid Organisation ID format. Must be a valid GUID.");
        }

        var accounts = await accountsService.GetAllAsync(request, parsedOrganisationId);
        if (!accounts.Records.Any())
        {
            return NoContent();
        }

        return Ok(accounts);
    }

    [HttpGet("{id:guid}")]
    [ActionName(nameof(GetByIdAsync))]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var user = await accountsService.GetByIdAsync(id);
        if (user == null)
        {
            return NoContent();
        }

        return Ok(user);
    }

    [HttpPost("Create")]
    [ActionName(nameof(CreateAsync))]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> CreateAsync([FromBody] CreatePersonRequest createPersonRequest)
    {
        var createdAccount = await accountsService.CreateAsync(createPersonRequest.ToPerson());

        return CreatedAtAction(
            nameof(GetByIdAsync),
            new { id = createdAccount.PersonId },
            createdAccount
        );
    }

    [HttpPut]
    [ActionName(nameof(UpdateAsync))]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> UpdateAsync([FromBody] UpdatePersonRequest updatePersonRequest)
    {
        var updatedAccount = await accountsService.UpdateAsync(updatePersonRequest.ToPerson());

        if (updatedAccount is null)
        {
            return BadRequest("Account not found.");
        }

        return Ok(updatedAccount);
    }


    [HttpDelete("{id:guid}")]
    [ActionName(nameof(DeleteAsync))]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
    {
        var deletedAccount = await accountsService.DeleteAsync(id);

        if (deletedAccount is null)
        {
            return BadRequest("Account not found.");
        }

        return NoContent();
    }

    [HttpGet("{id:guid}/linking-token")]
    [ActionName(nameof(GetLinkingTokenByIdAsync))]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetLinkingTokenByIdAsync(Guid id)
    {
        if (await accountsService.GetByIdAsync(id) == null)
        {
            return NoContent();
        }

        var linkingToken = await oneLoginAccountLinkingService.GetLinkingTokenForAccountIdAsync(id);
        var result = new LinkingTokenResult { LinkingToken = linkingToken };

        return Ok(result);
    }


    [HttpPost("check")]
    [ActionName(nameof(CheckEmailExists))]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> CheckEmailExists([FromBody] CheckEmailRequest checkEmailRequest)
    {
        var user = await accountsService.GetByEmailAsync(checkEmailRequest.Email);
        return Ok(user != null);
    }

    [HttpGet("social-work-england-number/{socialWorkerEnglandNumber}")]
    [ActionName(nameof(GetBySocialWorkEnglandNumberAsync))]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetBySocialWorkEnglandNumberAsync([FromRoute] string socialWorkerEnglandNumber)
    {
        var user = await accountsService.GetBySocialWorkEnglandNumberAsync(socialWorkerEnglandNumber);

        return user is null
            ? NoContent()
            : Ok(user);
    }

    [AllowAnonymous]
    [HttpGet("version")]
    [ActionName(nameof(GetVersion))]
    [Produces(MediaTypeNames.Text.Plain)]
    public IActionResult GetVersion()
    {
        return Ok(appInfo.Version);
    }

    [PublicAPI]
    public record LinkingTokenResult
    {
        public required string LinkingToken { get; init; }
    }
}
