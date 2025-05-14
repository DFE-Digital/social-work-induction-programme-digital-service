using System.ComponentModel.DataAnnotations;
using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

[AuthorizeRoles(RoleType.Coordinator)]
public class SelectAccountType(
    ICreateAccountJourneyService createAccountJourneyService,
    EcfLinkGenerator linkGenerator
) : BasePageModel
{
    [BindProperty]
    [Required(ErrorMessage = "Select the type of user you want to add")]
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
}
