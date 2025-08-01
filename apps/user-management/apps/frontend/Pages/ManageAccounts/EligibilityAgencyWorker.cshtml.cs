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
/// Eligibility Agency Worker View Model
/// </summary>
[AuthorizeRoles(RoleType.Coordinator)]
public class EligibilityAgencyWorker(
    ICreateAccountJourneyService createAccountJourneyService,
    EcfLinkGenerator linkGenerator,
    IValidator<EligibilityAgencyWorker> validator)
    : BasePageModel
{
    [BindProperty] public bool? IsAgencyWorker { get; set; }

    public PageResult OnGet()
    {
        BackLinkPath = FromChangeLink ? linkGenerator.ManageAccount.ConfirmAccountDetails() : linkGenerator.ManageAccount.EligibilityStatutoryWork();
        IsAgencyWorker = createAccountJourneyService.GetIsAgencyWorker();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var validationResult = await validator.ValidateAsync(this);
        if (IsAgencyWorker is null || !validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            BackLinkPath = linkGenerator.ManageAccount.EligibilityStatutoryWork();
            return Page();
        }

        createAccountJourneyService.SetIsAgencyWorker(IsAgencyWorker);

        if (IsAgencyWorker is true)
        {
            createAccountJourneyService.SetIsRecentlyQualified(null);
            return Redirect(linkGenerator.ManageAccount.EligibilityFundingNotAvailable());
        }

        return Redirect(linkGenerator.ManageAccount.EligibilityQualification());
    }

    public PageResult OnGetChange()
    {
        FromChangeLink = true;
        return OnGet();
    }
}
