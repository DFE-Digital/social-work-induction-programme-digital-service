using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

public class EligibilityStatutoryWork(
    ICreateAccountJourneyService createAccountJourneyService,
    EcfLinkGenerator linkGenerator,
    IValidator<EligibilityStatutoryWork> validator)
    : ManageAccountsBasePageModel
{
    [BindProperty] public bool? IsStatutoryWorker { get; set; }

    public PageResult OnGet()
    {
        BackLinkPath = FromChangeLink ? linkGenerator.ManageAccount.ConfirmAccountDetails(OrganisationId) : linkGenerator.ManageAccount.EligibilitySocialWorkEngland(OrganisationId);
        IsStatutoryWorker = createAccountJourneyService.GetIsStatutoryWorker();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var validationResult = await validator.ValidateAsync(this);
        if (IsStatutoryWorker is null || !validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            BackLinkPath = linkGenerator.ManageAccount.EligibilitySocialWorkEngland(OrganisationId);
            return Page();
        }

        createAccountJourneyService.SetIsStatutoryWorker(IsStatutoryWorker);

        if (FromChangeLink)
        {
            if (IsStatutoryWorker == true)
            {
                return Redirect(linkGenerator.ManageAccount.ConfirmAccountDetails(OrganisationId));
            }
            return Redirect(linkGenerator.ManageAccount.EligibilityStatutoryWorkDropoutChange(OrganisationId));
        }
        return Redirect(IsStatutoryWorker is false
            ? linkGenerator.ManageAccount.EligibilityStatutoryWorkDropout(OrganisationId)
            : linkGenerator.ManageAccount.EligibilityAgencyWorker(OrganisationId));
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
