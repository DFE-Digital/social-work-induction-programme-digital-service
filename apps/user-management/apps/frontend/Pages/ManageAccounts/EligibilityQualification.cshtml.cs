using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

public class EligibilityQualification(
    ICreateAccountJourneyService createAccountJourneyService,
    EcfLinkGenerator linkGenerator,
    IValidator<EligibilityQualification> validator)
    : BasePageModel
{
    /// <summary>
    /// Property capturing whether the user has completed their social work qualification within the last 3 years.
    /// </summary>
    [BindProperty] public bool? IsRecentlyQualified { get; set; }

    public PageResult OnGet()
    {
        BackLinkPath = FromChangeLink ? linkGenerator.ManageAccount.EligibilityAgencyWorkerChange() : linkGenerator.ManageAccount.EligibilityAgencyWorker();
        IsRecentlyQualified = createAccountJourneyService.GetIsRecentlyQualified();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var validationResult = await validator.ValidateAsync(this);
        if (IsRecentlyQualified is null || !validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            BackLinkPath = linkGenerator.ManageAccount.EligibilityAgencyWorker();
            return Page();
        }

        createAccountJourneyService.SetIsRecentlyQualified(IsRecentlyQualified);

        return Redirect(IsRecentlyQualified is false
            ? linkGenerator.ManageAccount.EligibilityFundingNotAvailable()
            : linkGenerator.ManageAccount.EligibilityFundingAvailable());
    }

    public PageResult OnGetChange()
    {
        FromChangeLink = true;
        return OnGet();
    }
}
