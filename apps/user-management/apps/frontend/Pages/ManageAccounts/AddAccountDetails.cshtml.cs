using System.ComponentModel.DataAnnotations;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

/// <summary>
/// Add User Details View Model
/// </summary>
public class AddAccountDetails(
    ICreateAccountJourneyService createAccountJourneyService,
    IValidator<AccountDetails> validator
) : PageModel
{
    /// <summary>
    /// First Name
    /// </summary>
    [BindProperty]
    [Display(Name = "First name")]
    public string? FirstName { get; set; }

    /// <summary>
    /// Last Name
    /// </summary>
    [BindProperty]
    [Display(Name = "Last name")]
    public string? LastName { get; set; }

    /// <summary>
    /// Email
    /// </summary>
    [BindProperty]
    [Display(Name = "Email address")]
    public string? Email { get; set; }

    /// <summary>
    /// Social Work England number
    /// </summary>
    [BindProperty]
    [Display(Name = "Social Work England number")]
    public string? SocialWorkEnglandNumber { get; set; }

    public PageResult OnGet()
    {
        var accountDetails = createAccountJourneyService.GetAccountDetails();

        FirstName = accountDetails?.FirstName;
        LastName = accountDetails?.LastName;
        Email = accountDetails?.Email;
        SocialWorkEnglandNumber = accountDetails?.SocialWorkEnglandNumber;

        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        var accountDetails = new AccountDetails
        {
            FirstName = FirstName,
            LastName = LastName,
            Email = Email,
            SocialWorkEnglandNumber = SocialWorkEnglandNumber
        };
        var result = await validator.ValidateAsync(accountDetails);
        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            return Page();
        }

        createAccountJourneyService.SetAccountDetails(accountDetails);

        return RedirectToPage(nameof(ConfirmAccountDetails));
    }
}
