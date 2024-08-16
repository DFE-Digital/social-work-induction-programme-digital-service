using System.ComponentModel.DataAnnotations;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Repositories.Interfaces;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

public class ConfirmAccountDetails(
    ICreateAccountJourneyService createAccountJourneyService,
    IAccountRepository accountRepository,
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
        BackLinkPath = linkGenerator.EditAccountDetails(id);
        ChangeDetailsLink = linkGenerator.EditAccountDetailsChange(id);

        IsUpdatingAccount = true;
        Id = id;
        var account = accountRepository.GetById(id);

        if (account is null)
        {
            return NotFound();
        }

        var updatedAccountDetails = TempData.Peek<AccountDetails>("UpdatedAccountDetails-" + id);

        FirstName = updatedAccountDetails?.FirstName;
        LastName = updatedAccountDetails?.LastName;
        Email = updatedAccountDetails?.Email;
        SocialWorkEnglandNumber = updatedAccountDetails?.SocialWorkEnglandNumber;

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
        var existingAccount = accountRepository.GetById(id);
        if (existingAccount is null)
        {
            return NotFound();
        }

        var updatedAccountDetails = TempData.Get<AccountDetails>("UpdatedAccountDetails-" + id);
        if (updatedAccountDetails is null)
        {
            return BadRequest("Updated account details are missing from session.");
        }

        var updatedAccount = new Account(existingAccount)
        {
            FirstName = updatedAccountDetails.FirstName,
            LastName = updatedAccountDetails.LastName,
            Email = updatedAccountDetails.Email,
            SocialWorkEnglandNumber = updatedAccountDetails.SocialWorkEnglandNumber
        };

        accountRepository.Update(updatedAccount);

        return Redirect(linkGenerator.ViewAccountDetails(id));
    }
}
