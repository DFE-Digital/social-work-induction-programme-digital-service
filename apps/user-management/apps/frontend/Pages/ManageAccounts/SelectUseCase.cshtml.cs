using System.ComponentModel.DataAnnotations;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

/// <summary>
/// Select Use Case Model
/// </summary>
public class SelectUseCase(ICreateAccountJourneyService createAccountJourneyService) : PageModel
{
    /// <summary>
    /// Selected Account Types
    /// </summary>
    [BindProperty]
    [Required(ErrorMessage = "Select what they need to do")]
    public IList<AccountType>? SelectedAccountTypes { get; set; }

    public PageResult OnGet()
    {
        return Page();
    }

    public IActionResult OnPost()
    {
        if (SelectedAccountTypes is null)
        {
            return Page();
        }

        createAccountJourneyService.SetAccountTypes(SelectedAccountTypes);

        return RedirectToPage(nameof(AddAccountDetails));
    }
}
