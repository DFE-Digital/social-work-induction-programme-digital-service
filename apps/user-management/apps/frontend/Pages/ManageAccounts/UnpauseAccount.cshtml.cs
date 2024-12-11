using System.Security.Claims;
using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

[AuthorizeRoles(RoleType.Coordinator)]
public class UnpauseAccount(
    IEditAccountJourneyService editAccountJourneyService,
    EcfLinkGenerator linkGenerator,
    IEmailService emailService
) : BasePageModel
{
    public Guid Id { get; set; }

    [BindProperty]
    public AccountDetails? AccountDetails { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var accountDetails = await editAccountJourneyService.GetAccountDetailsAsync(id);
        if (accountDetails is null)
        {
            return NotFound();
        }

        BackLinkPath ??= linkGenerator.ViewAccountDetails(id);
        Id = id;
        AccountDetails = accountDetails;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(Guid id)
    {
        var accountDetails = await editAccountJourneyService.GetAccountDetailsAsync(id);
        var accountTypes = await editAccountJourneyService.GetAccountTypesAsync(id);
        if (accountDetails is null || accountTypes is null)
        {
            return NotFound();
        }

        var statusValue = AccountStatus.Active;
        if (await editAccountJourneyService.GetIsStaffAsync(id) == false)
        {
            statusValue = string.IsNullOrEmpty(accountDetails.SocialWorkEnglandNumber)
                ? AccountStatus.PendingRegistration
                : statusValue;
        }

        var emailSuccessful = await emailService.UnpauseAccountAsync(
            accountDetails,
            accountTypes,
            User.Identity!.Name,
            User.GetClaim(ClaimTypes.Email)
        );

        if (emailSuccessful)
        {
            TempData["NotifyEmail"] = accountDetails.Email;
            TempData["NotificationBannerSubject"] = "Account was successfully paused";
        }

        await editAccountJourneyService.SetAccountStatusAsync(id, statusValue);
        await editAccountJourneyService.CompleteJourneyAsync(id);

        return Redirect(linkGenerator.ManageAccounts());
    }
}
