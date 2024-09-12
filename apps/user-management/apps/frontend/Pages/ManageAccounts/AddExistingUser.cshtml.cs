using System.ComponentModel.DataAnnotations;
using Dfe.Sww.Ecf.Frontend.Models.NameMatch;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

public class AddExistingUser(
    ICreateAccountJourneyService createAccountJourneyService,
    EcfLinkGenerator linkGenerator,
    ISocialWorkEnglandService socialWorkEnglandService
) : BasePageModel
{
    /// <summary>
    /// Name
    /// </summary>
    [Display(Name = "Name")]
    public string? Name { get; set; }

    /// <summary>
    /// Email
    /// </summary>
    [Display(Name = "Email")]
    public string? Email { get; set; }

    /// <summary>
    /// Social Work England number
    /// </summary>
    [Display(Name = "Social Work England registration number")]
    public string? SocialWorkEnglandNumber { get; set; }

    /// <summary>
    /// Score representing how closely the name entered matches an existing Social Worker record
    /// </summary>
    public MatchResult? NameMatchResult { get; set; }

    private void SetBackLinkPath()
    {
        BackLinkPath ??= linkGenerator.AddAccountDetails();
    }

    /// <summary>
    /// Action for confirming user details
    /// </summary>
    /// <returns>A confirmation screen displaying user details</returns>
    public PageResult OnGet()
    {
        BackLinkPath = linkGenerator.AddAccountDetails();

        var accountDetails = createAccountJourneyService.GetAccountDetails();

        var socialWorkerDetails = createAccountJourneyService.GetSocialWorkerDetails();
        if (socialWorkerDetails is null || accountDetails is null)
        {
            return new PageResult();
        }

        Name = socialWorkerDetails.RegisteredName;
        SocialWorkEnglandNumber = socialWorkerDetails.Id;
        Email = accountDetails.Email;
        NameMatchResult = socialWorkEnglandService.GetNameMatchScore(
            accountDetails.FirstName,
            accountDetails.LastName,
            socialWorkerDetails.RegisteredName
        );

        return Page();
    }

    /// <summary>
    /// Action for confirming user details
    /// </summary>
    /// <returns>A confirmation screen displaying user details</returns>
    public RedirectResult OnPost()
    {
        var accountDetails = createAccountJourneyService.GetAccountDetails();
        createAccountJourneyService.CompleteJourney();

        TempData["NotifyEmail"] = accountDetails?.Email;
        TempData["NotificationBannerSubject"] = "Account was successfully added";

        return Redirect(linkGenerator.ManageAccounts());
    }
}
