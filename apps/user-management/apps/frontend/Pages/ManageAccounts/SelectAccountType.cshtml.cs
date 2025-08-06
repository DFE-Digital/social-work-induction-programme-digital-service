using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

public class SelectAccountType(
    ICreateAccountJourneyService createAccountJourneyService,
    EcfLinkGenerator linkGenerator,
    IValidator<SelectAccountType> validator
) : ManageAccountsBasePageModel
{
    [BindProperty]
    public bool? IsStaff { get; set; }

    public Guid? EditAccountId { get; set; }

    public string? Handler { get; set; }

    private void SetBackLinkPath()
    {
        BackLinkPath = FromChangeLink
            ? linkGenerator.ManageAccount.ConfirmAccountDetails(OrganisationId)
            : linkGenerator.ManageAccount.Index(OrganisationId);
    }

    private string GetRedirectPath()
    {
        var details = createAccountJourneyService.GetAccountDetails();

        if (IsStaff == true)
        {
            return FromChangeLink ? linkGenerator.ManageAccount.SelectUseCaseChange(OrganisationId) : linkGenerator.ManageAccount.SelectUseCase(OrganisationId);
        }

        if (FromChangeLink)
        {
            if (createAccountJourneyService.GetIsRegisteredWithSocialWorkEngland() is null)
            {
                return linkGenerator.ManageAccount.EligibilityInformation(OrganisationId);
            }
            if (details?.SocialWorkEnglandNumber is null)
            {
                return linkGenerator.ManageAccount.AddAccountDetailsChange(OrganisationId);
            }
            return linkGenerator.ManageAccount.ConfirmAccountDetails(OrganisationId);
        }

        return linkGenerator.ManageAccount.EligibilityInformation(OrganisationId);
    }

    private void SetHandler()
    {
        Handler = EditAccountId is not null ? "edit" : (FromChangeLink ? "change" : "");
    }

    public RedirectResult OnGetNew()
    {
        SetHandler();
        createAccountJourneyService.ResetCreateAccountJourneyModel();
        return Redirect(linkGenerator.ManageAccount.SelectAccountType(OrganisationId));
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
