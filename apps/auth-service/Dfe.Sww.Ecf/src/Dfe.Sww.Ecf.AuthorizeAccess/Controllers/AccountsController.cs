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
    public async Task<IActionResult> GetAllAsync()
    {
        var accounts = await accountsService.GetAllAsync();
        if (!accounts.Any())
        {
            return NoContent();
        }

        return Ok(accounts);
    }

    [HttpGet("{id:guid}")]
    [Produces("application/json")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var user = await accountsService.GetByIdAsync(id);
        if (user == null)
        {
            return NoContent();
        }

        return Ok(user);
    }

    [HttpGet("{id:guid}/linking-token")]
    [Produces("application/json")]
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
