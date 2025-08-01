using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

/// <summary>
/// Eligibility Statutory Work View Model
/// </summary>
[AuthorizeRoles(RoleType.Coordinator)]
public class EligibilityStatutoryWork(
    ICreateAccountJourneyService createAccountJourneyService,
    EcfLinkGenerator linkGenerator,
    IValidator<EligibilityStatutoryWork> validator)
    : BasePageModel
{
    [BindProperty] public bool? IsStatutoryWorker { get; set; }

    public PageResult OnGet()
    {
        BackLinkPath = FromChangeLink ? linkGenerator.ManageAccount.ConfirmAccountDetails() : linkGenerator.ManageAccount.EligibilitySocialWorkEngland();
        IsStatutoryWorker = createAccountJourneyService.GetIsStatutoryWorker();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var validationResult = await validator.ValidateAsync(this);
        if (IsStatutoryWorker is null || !validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            BackLinkPath = linkGenerator.ManageAccount.EligibilitySocialWorkEngland();
            return Page();
        }

        createAccountJourneyService.SetIsStatutoryWorker(IsStatutoryWorker);

        if (FromChangeLink)
        {
            if (IsStatutoryWorker == true)
            {
                return Redirect(linkGenerator.ManageAccount.ConfirmAccountDetails());
            }
            return Redirect(linkGenerator.ManageAccount.EligibilityStatutoryWorkDropoutChange());
        }
        return Redirect(IsStatutoryWorker is false
            ? linkGenerator.ManageAccount.EligibilityStatutoryWorkDropout()
            : linkGenerator.ManageAccount.EligibilityAgencyWorker());
    }

    public PageResult OnGetChange()
    {
        FromChangeLink = true;
        return OnGet();
    }

    public async Task<IActionResult> OnPostChangeAsync()
    {
        FromChangeLink = true;
        return await OnPostAsync();
    }
}
