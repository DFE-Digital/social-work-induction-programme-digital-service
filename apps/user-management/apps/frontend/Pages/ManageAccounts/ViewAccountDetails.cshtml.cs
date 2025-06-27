using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

[AuthorizeRoles(RoleType.Coordinator)]
public class ViewAccountDetails(
    IAccountService accountService,
    EcfLinkGenerator linkGenerator,
    ICreateAccountJourneyService createAccountJourneyService
)
    : BasePageModel
{
    public Guid Id { get; set; }
    public Account Account { get; set; } = default!;

    public bool IsSocialWorker { get; set; }

    public bool IsAssessor { get; set; }

    public bool HasCompletedLoginAccountLinking { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var account = await accountService.GetByIdAsync(id);
        if (account is null) return NotFound();

        BackLinkPath = linkGenerator.ManageAccounts();
        Account = account;

        if (Account.Types is null) return Page();

        IsSocialWorker = Account.Types.Contains(AccountType.EarlyCareerSocialWorker);
        IsAssessor = Account.Types.Contains(AccountType.Assessor);
        HasCompletedLoginAccountLinking = Account.HasCompletedLoginAccountLinking;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(Guid id)
    {
        var account = await accountService.GetByIdAsync(id);
        if (account is null) return NotFound();
        await createAccountJourneyService.SendInvitationEmailAsync(account);
        return Redirect(linkGenerator.ManageAccounts());
    }
}
