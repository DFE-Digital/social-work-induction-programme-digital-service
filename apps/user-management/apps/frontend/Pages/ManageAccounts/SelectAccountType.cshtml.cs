using System.ComponentModel.DataAnnotations;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

/// <summary>
/// Select User Type Model
/// </summary>
public class SelectAccountType(ICreateAccountJourneyService createAccountJourneyService) : PageModel
{
    /// <summary>
    /// Selected Account Type
    /// </summary>
    [BindProperty]
    [Required(ErrorMessage = "Select who you want to add")]
    public AccountType? SelectedAccountType { get; set; }

    public PageResult OnGet()
    {
        return Page();
    }

    public IActionResult OnPost()
    {
        switch (SelectedAccountType)
        {
            case null:
                return Page();
            case AccountType.AssessorCoordinator:
                return RedirectToPage(nameof(SelectUseCase));
            case AccountType.Coordinator:
            case AccountType.Assessor:
            case AccountType.EarlyCareerSocialWorker:
                var accountTypes = new List<AccountType> { SelectedAccountType.Value };
                createAccountJourneyService.SetAccountTypes(accountTypes);

                return RedirectToPage(nameof(AddAccountDetails));
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
