using System.ComponentModel.DataAnnotations;
using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageUsers;

[AuthorizeRoles(RoleType.Coordinator)]
public class SelectUserType(
    ICreateUserJourneyService createUserJourneyService,
    EcfLinkGenerator linkGenerator,
    IValidator<SelectUserType> validator
) : BasePageModel
{
    [BindProperty]
    public bool? IsStaff { get; set; }

    public Guid? EditUserId { get; set; }

    public RedirectResult OnGetNew()
    {
        createUserJourneyService.ResetCreateUserJourneyModel();
        return Redirect(linkGenerator.SelectUserType());
    }

    public PageResult OnGet()
    {
        BackLinkPath = linkGenerator.ManageUsers();
        IsStaff = createUserJourneyService.GetIsStaff();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var validationResult = await validator.ValidateAsync(this);
        if (IsStaff is null || !validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            BackLinkPath = linkGenerator.ManageUsers();
            return Page();
        }

        createUserJourneyService.SetIsStaff(IsStaff);

        if (IsStaff is true)
        {
            return Redirect(linkGenerator.SelectUseCase());
        }
        createUserJourneyService.SetUserTypes([UserType.EarlyCareerSocialWorker]);
        return Redirect(linkGenerator.EligibilityInformation());
    }
}
