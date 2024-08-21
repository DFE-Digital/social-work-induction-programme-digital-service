using System.ComponentModel.DataAnnotations;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

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

    public IActionResult OnGetUpdate(Guid id)
    {
        if (!editAccountJourneyService.IsAccountIdValid(id))
        {
            return NotFound();
        }

        BackLinkPath = linkGenerator.EditAccountDetails(id);
        ChangeDetailsLink = linkGenerator.EditAccountDetailsChange(id);

        IsUpdatingAccount = true;
        Id = id;

        var updatedAccountDetails = editAccountJourneyService.GetAccountDetails(id);

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
    public RedirectResult OnPost()
    {
        var accountDetails = createAccountJourneyService.GetAccountDetails();
        createAccountJourneyService.CompleteJourney();

        TempData["CreatedAccountEmail"] = accountDetails?.Email;

        return Redirect(linkGenerator.ManageAccounts());
    }

    public IActionResult OnPostUpdate(Guid id)
    {
        if (!editAccountJourneyService.IsAccountIdValid(id))
        {
            return NotFound();
        }

        editAccountJourneyService.CompleteJourney(id);

        return Redirect(linkGenerator.ViewAccountDetails(id));
    }
}
