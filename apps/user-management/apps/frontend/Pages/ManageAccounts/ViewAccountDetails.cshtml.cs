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

    public bool IsSocialWorker { get; set; }

    public bool IsAssessor { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var account = await accountService.GetByIdAsync(id);
        if (account is null) return NotFound();

        BackLinkPath = linkGenerator.ManageAccounts();
        Account = account;

        if (Account.Types is null) return Page();

        IsSocialWorker = Account.Types.Contains(AccountType.EarlyCareerSocialWorker);
        IsAssessor = Account.Types.Contains(AccountType.Assessor);

        return Page();
    }
}
