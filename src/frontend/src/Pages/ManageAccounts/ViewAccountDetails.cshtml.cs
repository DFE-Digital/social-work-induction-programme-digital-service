using Microsoft.AspNetCore.Mvc;
using SocialWorkInductionProgramme.Frontend.Authorisation;
using SocialWorkInductionProgramme.Frontend.Models;
using SocialWorkInductionProgramme.Frontend.Pages.Shared;
using SocialWorkInductionProgramme.Frontend.Routing;
using SocialWorkInductionProgramme.Frontend.Services.Interfaces;

namespace SocialWorkInductionProgramme.Frontend.Pages.ManageAccounts;

[AuthorizeRoles(RoleType.Coordinator)]
public class ViewAccountDetails(IAccountService accountService, EcfLinkGenerator linkGenerator)
    : BasePageModel
{
    public Account Account { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var account = await accountService.GetByIdAsync(id);
        if (account is null)
        {
            return NotFound();
        }

        BackLinkPath = linkGenerator.ManageAccounts();
        Account = account;

        return Page();
    }
}
