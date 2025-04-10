using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SocialWorkInductionProgramme.Frontend.Authorisation;
using SocialWorkInductionProgramme.Frontend.Pages.Shared;
using SocialWorkInductionProgramme.Frontend.Routing;
using SocialWorkInductionProgramme.Frontend.Services.Journeys.Interfaces;

namespace SocialWorkInductionProgramme.Frontend.Pages.ManageAccounts;

[AuthorizeRoles(RoleType.Coordinator)]
public class ConfirmAccountDetails(
    ICreateAccountJourneyService createAccountJourneyService,
    IEditAccountJourneyService editAccountJourneyService,
    EcfLinkGenerator linkGenerator
) : BasePageModel
{
    public Guid Id { get; set; }

    /// <summary>
    /// First Name
    /// </summary>
    [Display(Name = "First name")]
    public string? FirstName { get; set; }

    /// <summary>
    /// Last Name
    /// </summary>
    [Display(Name = "Last name")]
    public string? LastName { get; set; }

    /// <summary>
    /// Email
    /// </summary>
    [Display(Name = "Email address")]
    public string? Email { get; set; }

    /// <summary>
    /// Social Work England number
    /// </summary>
    [Display(Name = "Social Work England number")]
    public string? SocialWorkEnglandNumber { get; set; }

    public string? ChangeDetailsLink { get; set; }

    public bool IsUpdatingAccount { get; set; }

    /// <summary>
    /// Action for confirming user details
    /// </summary>
    /// <returns>A confirmation screen displaying user details</returns>
    public PageResult OnGet()
    {
        BackLinkPath = linkGenerator.AddAccountDetails();
        ChangeDetailsLink = linkGenerator.AddAccountDetailsChange();

        var accountDetails = createAccountJourneyService.GetAccountDetails();

        FirstName = accountDetails?.FirstName;
        LastName = accountDetails?.LastName;
        Email = accountDetails?.Email;
        SocialWorkEnglandNumber = accountDetails?.SocialWorkEnglandNumber;

        return Page();
    }

    public async Task<IActionResult> OnGetUpdateAsync(Guid id)
    {
        var updatedAccountDetails = await editAccountJourneyService.GetAccountDetailsAsync(id);
        if (updatedAccountDetails is null)
        {
            return NotFound();
        }

        BackLinkPath = linkGenerator.EditAccountDetails(id);
        ChangeDetailsLink = linkGenerator.EditAccountDetailsChange(id);

        IsUpdatingAccount = true;
        Id = id;

        FirstName = updatedAccountDetails.FirstName;
        LastName = updatedAccountDetails.LastName;
        Email = updatedAccountDetails.Email;
        SocialWorkEnglandNumber = updatedAccountDetails.SocialWorkEnglandNumber;

        return Page();
    }

    /// <summary>
    /// Action for confirming user details
    /// </summary>
    /// <returns>A confirmation screen displaying user details</returns>
    public async Task<RedirectResult> OnPostAsync()
    {
        var accountDetails = createAccountJourneyService.GetAccountDetails();
        await createAccountJourneyService.CompleteJourneyAsync();

        TempData["NotifyEmail"] = accountDetails?.Email;
        TempData["NotificationBannerSubject"] = "Account was successfully added";

        return Redirect(linkGenerator.ManageAccounts());
    }

    public async Task<IActionResult> OnPostUpdateAsync(Guid id)
    {
        if (!await editAccountJourneyService.IsAccountIdValidAsync(id))
        {
            return NotFound();
        }

        await editAccountJourneyService.CompleteJourneyAsync(id);

        return Redirect(linkGenerator.ViewAccountDetails(id));
    }
}
