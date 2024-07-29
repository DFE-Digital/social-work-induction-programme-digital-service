using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

public class ViewAccountDetails(IAccountRepository accountRepository) : PageModel
{
    public Account Account { get; set; } = default!;

    public IActionResult OnGet(Guid? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var account = accountRepository.GetById(id);

        if (account is null)
        {
            return NotFound();
        }

        Account = account;

        return Page();
    }
}
