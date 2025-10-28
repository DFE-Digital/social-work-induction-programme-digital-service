using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

public class EligibilityAgencyWorker(
    ICreateAccountJourneyService createAccountJourneyService,
    EcfLinkGenerator linkGenerator,
    IValidator<EligibilityAgencyWorker> validator)
    : ManageAccountsBasePageModel
{
    [BindProperty] public bool? IsAgencyWorker { get; set; }

    public PageResult OnGet()
    {
        var isEnrolledInAsye = createAccountJourneyService.GetIsEnrolledInAsye();
        var nonChangeLinkBackPath = isEnrolledInAsye == true
            ? linkGenerator.ManageAccount.EligibilitySocialWorkEnglandAsyeDropout()
            : linkGenerator.ManageAccount.EligibilitySocialWorkEngland(OrganisationId);

        BackLinkPath = FromChangeLink
            ? linkGenerator.ManageAccount.ConfirmAccountDetails(OrganisationId)
            : nonChangeLinkBackPath;


        IsAgencyWorker = createAccountJourneyService.GetIsAgencyWorker();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var validationResult = await validator.ValidateAsync(this);
        if (IsAgencyWorker is null || !validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            BackLinkPath = FromChangeLink ? linkGenerator.ManageAccount.ConfirmAccountDetails(OrganisationId) : linkGenerator.ManageAccount.EligibilitySocialWorkEngland(OrganisationId);
            return Page();
        }

        createAccountJourneyService.SetIsAgencyWorker(IsAgencyWorker);

        if (IsAgencyWorker is true)
        {
            createAccountJourneyService.SetIsRecentlyQualified(null);
            return Redirect(linkGenerator.ManageAccount.EligibilityFundingNotAvailable(OrganisationId));
        }

        return Redirect(linkGenerator.ManageAccount.EligibilityQualification(OrganisationId));
    }

    public PageResult OnGetChange()
    {
        FromChangeLink = true;
        return OnGet();
    }
}
