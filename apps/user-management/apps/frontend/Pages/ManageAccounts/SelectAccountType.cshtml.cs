using System.ComponentModel.DataAnnotations;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

public class SelectAccountType(ICreateAccountJourneyService createAccountJourneyService)
    : BasePageModel
{
    [BindProperty]
    [Required(ErrorMessage = "Select who you want to add")]
    public bool? IsStaff { get; set; }

    public IActionResult OnGetNew()
    {
        createAccountJourneyService.ResetCreateAccountJourneyModel();
        return RedirectToPage(nameof(SelectAccountType));
    }

    public PageResult OnGet()
    {
        IsStaff = createAccountJourneyService.GetIsStaff();
        return Page();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (IsStaff is false)
        {
            createAccountJourneyService.SetAccountTypes(
                new List<AccountType> { AccountType.EarlyCareerSocialWorker }
            );
        }

        createAccountJourneyService.SetIsStaff(IsStaff);

        return RedirectToPage(IsStaff is true ? nameof(SelectUseCase) : nameof(AddAccountDetails));
    }
}
