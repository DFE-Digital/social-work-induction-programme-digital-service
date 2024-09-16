using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Journeys;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

public class LinkAccount(
    IEditAccountJourneyService editAccountJourneyService,
    EcfLinkGenerator linkGenerator
) : BasePageModel
{
    public Guid Id { get; set; }

    [BindProperty]
    public string? Email { get; set; }

    public IActionResult OnGet(Guid id)
    {
        if (!editAccountJourneyService.IsAccountIdValid(id))
        {
            return NotFound();
        }

        BackLinkPath ??= linkGenerator.ViewAccountDetails(id);
        Id = id;

        var accountDetails = editAccountJourneyService.GetAccountDetails(id);
        Email = accountDetails.Email;

        return Page();
    }

    public IActionResult OnPost(Guid id)
    {
        if (!editAccountJourneyService.IsAccountIdValid(id))
        {
            return NotFound();
        }

        var accountDetails = editAccountJourneyService.GetAccountDetails(id);
        TempData["NotifyEmail"] = accountDetails.Email;
        TempData["NotificationBannerSubject"] = "Account was successfully linked to this organisation";

        var statusValue = AccountStatus.Active;
        if (editAccountJourneyService.GetIsStaff(id) == false)
        {
            statusValue = string.IsNullOrEmpty(accountDetails.SocialWorkEnglandNumber)
                ? AccountStatus.PendingRegistration
                : statusValue;
        }
        editAccountJourneyService.SetAccountStatus(id, statusValue);
        editAccountJourneyService.CompleteJourney(id);

        return Redirect(linkGenerator.ManageAccounts());
    }
}
