using System.ComponentModel.DataAnnotations;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

public class ConfirmAccountDetails(ICreateAccountJourneyService createAccountJourneyService)
    : PageModel
{
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

    /// <summary>
    /// Account Types
    /// </summary>
    public IList<AccountType>? AccountTypes { get; set; }

    /// <summary>
    /// Action for confirming user details
    /// </summary>
    /// <returns>A confirmation screen displaying user details</returns>
    public PageResult OnGet()
    {
        var accountDetails = createAccountJourneyService.GetAccountDetails();

        FirstName = accountDetails?.FirstName;
        LastName = accountDetails?.LastName;
        Email = accountDetails?.Email;
        SocialWorkEnglandNumber = accountDetails?.SocialWorkEnglandNumber;

        AccountTypes = createAccountJourneyService.GetAccountTypes();

        return Page();
    }

    /// <summary>
    /// Action for confirming user details
    /// </summary>
    /// <returns>A confirmation screen displaying user details</returns>
    public RedirectToPageResult OnPost()
    {
        var accountDetails = createAccountJourneyService.GetAccountDetails();
        createAccountJourneyService.CompleteJourney();

        TempData["CreatedAccountEmail"] = accountDetails?.Email;

        return RedirectToPage(nameof(Index));
    }
}
