using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Email;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using GovUk.Frontend.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

public class ViewAccountDetails(
    IAccountService accountService,
    EcfLinkGenerator linkGenerator,
    IEmailService emailService,
    IEditAccountJourneyService editAccountJourneyService
)
    : ManageAccountsBasePageModel
{
    public Account Account { get; set; } = default!;

    public bool IsSocialWorker { get; set; }

    public bool IsAssessor { get; set; }

    public bool HasCompletedLoginAccountLinking { get; set; }

    public async Task<RedirectResult> OnGetNewAsync(Guid id)
    {
        await editAccountJourneyService.ResetEditAccountJourneyModelAsync(id);
        return Redirect(linkGenerator.ManageAccount.ViewAccountDetails(id, OrganisationId));
    }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var account = await accountService.GetByIdAsync(id);
        if (account is null) return NotFound();

        BackLinkPath = linkGenerator.ManageAccount.Index(OrganisationId);
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
        await emailService.SendInvitationEmailAsync(new InvitationEmailRequest
        {
            AccountId = account.Id,
            OrganisationName = "Test Organisation", // TODO: Get organisation name
            Role = account.Types?.Min()
        });

        TempData["NotificationType"] = NotificationBannerType.Success;
        TempData["NotificationHeader"] = "An invitation to register has been resent";
        TempData["NotificationMessage"] = $"A new invitation to register has been sent to {account.FullName}, {account.Email}";

        return Redirect(linkGenerator.ManageAccount.Index(OrganisationId));
    }
}
