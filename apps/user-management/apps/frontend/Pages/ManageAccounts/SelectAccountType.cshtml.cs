using System.ComponentModel.DataAnnotations;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

public class SelectAccountType(
    ICreateAccountJourneyService createAccountJourneyService,
    EcfLinkGenerator linkGenerator,
    IEditAccountJourneyService editAccountJourneyService
) : BasePageModel
{
    [BindProperty]
    [Required(ErrorMessage = "Select who you want to add")]
    public bool? IsStaff { get; set; }

    public Guid? EditAccountId { get; set; }

    public RedirectResult OnGetNew()
    {
        createAccountJourneyService.ResetCreateAccountJourneyModel();
        return Redirect(linkGenerator.SelectAccountType());
    }

    public PageResult OnGet()
    {
        BackLinkPath = linkGenerator.ManageAccounts();
        IsStaff = createAccountJourneyService.GetIsStaff();
        return Page();
    }

    public IActionResult OnGetEdit(Guid id)
    {
        if (!editAccountJourneyService.IsAccountIdValid(id))
        {
            return NotFound();
        }

        BackLinkPath = linkGenerator.ViewAccountDetails(id);
        EditAccountId = id;

        return Page();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            BackLinkPath = linkGenerator.ManageAccounts();
            return Page();
        }

        createAccountJourneyService.SetIsStaff(IsStaff);

        if (IsStaff is true)
        {
            return Redirect(linkGenerator.SelectUseCase());
        }
        createAccountJourneyService.SetAccountTypes([AccountType.EarlyCareerSocialWorker]);
        return Redirect(linkGenerator.AddAccountDetails());
    }

    public IActionResult OnPostEdit(Guid id)
    {
        if (!ModelState.IsValid)
        {
            BackLinkPath = linkGenerator.ViewAccountDetails(id);
            return Page();
        }

        if (!editAccountJourneyService.IsAccountIdValid(id))
        {
            return NotFound();
        }

        editAccountJourneyService.SetIsStaff(id, IsStaff);

        if (IsStaff is true)
        {
            return Redirect(linkGenerator.EditUseCase(id));
        }

        editAccountJourneyService.SetAccountTypes(id, [AccountType.EarlyCareerSocialWorker]);
        editAccountJourneyService.CompleteJourney(id);
        return Redirect(linkGenerator.ViewAccountDetails(id));
    }
}
