using System.ComponentModel.DataAnnotations;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

public class UnlinkAccount(
    IEditAccountJourneyService editAccountJourneyService,
    EcfLinkGenerator linkGenerator
) : BasePageModel
{
    public Guid Id { get; set; }

    [BindProperty]
    public string? Email { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        if (!await editAccountJourneyService.IsAccountIdValidAsync(id))
        {
            return NotFound();
        }

        BackLinkPath ??= linkGenerator.ViewAccountDetails(id);
        Id = id;

        var accountDetails = await editAccountJourneyService.GetAccountDetailsAsync(id);
        Email = accountDetails.Email;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(Guid id)
    {
        if (!await editAccountJourneyService.IsAccountIdValidAsync(id))
        {
            return NotFound();
        }

        var accountDetails = await editAccountJourneyService.GetAccountDetailsAsync(id);
        TempData["NotifyEmail"] = accountDetails.Email;
        TempData["NotificationBannerSubject"] =
            "Account was successfully unlinked from this organisation";

        await editAccountJourneyService.SetAccountStatusAsync(id, AccountStatus.Inactive);
        await editAccountJourneyService.CompleteJourneyAsync(id);

        return Redirect(linkGenerator.ManageAccounts());
    }
}
