using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

public class Index(IAccountService accountService) : BasePageModel
{
    public IList<Account> Accounts { get; set; } = default!;

    public async Task<PageResult> OnGet()
    {
        Accounts = (await accountService.GetAllAsync())
            .OrderBy(account => account.CreatedAt)
            .ToList();

        return Page();
    }
}
