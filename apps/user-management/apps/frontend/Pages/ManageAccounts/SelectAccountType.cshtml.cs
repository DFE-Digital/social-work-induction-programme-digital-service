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

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

[AuthorizeRoles(RoleType.Coordinator)]
public class SelectAccountType(
    ICreateAccountJourneyService createAccountJourneyService,
    EcfLinkGenerator linkGenerator,
    IValidator<SelectAccountType> validator
) : BasePageModel
{
    [BindProperty]
    public bool? IsStaff { get; set; }

    public Guid? EditAccountId { get; set; }

    public string? Handler { get; set; }

    private void SetBackLinkPath()
    {
        BackLinkPath = FromChangeLink
            ? linkGenerator.ConfirmAccountDetails()
            : linkGenerator.ManageAccounts();
    }

    private string GetRedirectPath()
    {
        var details = createAccountJourneyService.GetAccountDetails();

        if (IsStaff == true)
        {
            return FromChangeLink ? linkGenerator.SelectUseCaseChange() : linkGenerator.SelectUseCase();
        }

        if (FromChangeLink)
        {
            if (createAccountJourneyService.GetIsRegisteredWithSocialWorkEngland() is null)
            {
                return linkGenerator.EligibilityInformation();
            }
            if (details?.SocialWorkEnglandNumber is null)
            {
                return linkGenerator.AddAccountDetailsChange();
            }
            return linkGenerator.ConfirmAccountDetails();
        }

        return linkGenerator.EligibilityInformation();
    }

    private void SetHandler()
    {
        Handler = EditAccountId is not null ? "edit" : (FromChangeLink ? "change" : "");
    }

    public RedirectResult OnGetNew()
    {
        SetHandler();
        createAccountJourneyService.ResetCreateAccountJourneyModel();
        return Redirect(linkGenerator.SelectAccountType());
    }

    public PageResult OnGet()
    {
        SetHandler();
        SetBackLinkPath();
        IsStaff = createAccountJourneyService.GetIsStaff();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        SetHandler();
        var validationResult = await validator.ValidateAsync(this);
        if (IsStaff is null || !validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            SetBackLinkPath();
            return Page();
        }

        createAccountJourneyService.SetIsStaff(IsStaff);

        if (IsStaff is false)
        {
            createAccountJourneyService.SetAccountTypes([AccountType.EarlyCareerSocialWorker]);
        }

        return Redirect(GetRedirectPath());
    }

    public PageResult OnGetChange()
    {
        FromChangeLink = true;
        return OnGet();
    }

    public async Task<IActionResult> OnPostChangeAsync()
    {
        FromChangeLink = true;
        SetBackLinkPath();
        return await OnPostAsync();
    }
}
