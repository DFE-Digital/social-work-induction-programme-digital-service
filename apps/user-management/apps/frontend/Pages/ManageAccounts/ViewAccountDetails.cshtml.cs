using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

[AuthorizeRoles(RoleType.Coordinator)]
public class ViewAccountDetails(IAccountService accountService, EcfLinkGenerator linkGenerator)
    : BasePageModel
{
    public Account Account { get; set; } = default!;

    public string? UnlinkPath { get; set; }
    public string? LinkPath { get; set; }
    public string? UnpausePath { get; set; }
    public string? PausePath { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var account = await accountService.GetByIdAsync(id);
        if (account is null)
        {
            return NotFound();
        }

        BackLinkPath = linkGenerator.ManageAccounts();
        UnlinkPath = linkGenerator.UnlinkAccount(id);
        LinkPath = linkGenerator.LinkAccount(id);
        UnpausePath = linkGenerator.UnpauseAccount(id);
        PausePath = linkGenerator.PauseAccount(id);
        Account = account;

        return Page();
    }
}
