using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Repositories.Interfaces;
using Dfe.Sww.Ecf.Frontend.Routing;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

public class ViewAccountDetails(
    IAccountRepository accountRepository,
    EcfLinkGenerator linkGenerator
) : BasePageModel
{
    public Account Account { get; set; } = default!;

    public string? UnlinkPath { get; set; }

    public IActionResult OnGet(Guid id)
    {
        BackLinkPath = linkGenerator.ManageAccounts();
        UnlinkPath = linkGenerator.UnlinkAccount(id);

        var account = accountRepository.GetById(id);
        if (account is null)
        {
            return NotFound();
        }

        Account = account;

        return Page();
    }
}
