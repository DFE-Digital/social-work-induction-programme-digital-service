using System.Net.Mime;
using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using Dfe.Sww.Ecf.Core.Models.Pagination;
using Dfe.Sww.Ecf.Core.Services.Accounts;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Controllers;

[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/[controller]")]
public class AccountsController(
    IAccountsService accountsService,
    IOneLoginAccountLinkingService oneLoginAccountLinkingService
) : Controller
{
    [HttpGet]
    [ActionName(nameof(GetAllAsync))]
    public async Task<IActionResult> GetAllAsync([FromQuery] PaginationRequest request)
    {
        var accounts = await accountsService.GetAllAsync(request);
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
    public async Task<IActionResult> CreateAsync([FromBody] Person user)
    {
        var createdAccount = await accountsService.CreateAsync(user);
        return CreatedAtAction(
            nameof(GetByIdAsync),
            new { id = createdAccount.PersonId },
            createdAccount
        );
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

    [PublicAPI]
    public record LinkingTokenResult
    {
        public required string LinkingToken { get; init; }
    }
}
