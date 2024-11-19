using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

public class ViewAccountDetails(IAccountService accountService, EcfLinkGenerator linkGenerator)
    : BasePageModel
{
    public Account Account { get; set; } = default!;

    public string? UnlinkPath { get; set; }
    public string? LinkPath { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        BackLinkPath = linkGenerator.ManageAccounts();
        UnlinkPath = linkGenerator.UnlinkAccount(id);
        LinkPath = linkGenerator.LinkAccount(id);

        Account = await accountService.GetByIdAsync(id);

        return Page();
    }
}
