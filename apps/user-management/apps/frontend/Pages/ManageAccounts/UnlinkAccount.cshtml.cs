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
        TempData["notificationBannerSubject"] = "Account was successfully unlinked from this organisation";

        editAccountJourneyService.SetAccountStatus(id, AccountStatus.Inactive);
        editAccountJourneyService.CompleteJourney(id);

        return Redirect(linkGenerator.ManageAccounts());
    }
}
