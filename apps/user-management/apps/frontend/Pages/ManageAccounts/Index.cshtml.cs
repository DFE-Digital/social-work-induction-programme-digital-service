using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

public class Index(IAccountRepository accountRepository) : BasePageModel
{
    public IList<Account> Accounts { get; set; } = default!;

    public PageResult OnGet()
    {
        Accounts = accountRepository.GetAll();

        return Page();
    }
}
