using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageUsers;

/// <summary>
/// Eligibility Agency Worker View Model
/// </summary>
[AuthorizeRoles(RoleType.Coordinator)]
public class EligibilityAgencyWorker(
    ICreateUserJourneyService createUserJourneyService,
    EcfLinkGenerator linkGenerator,
    IValidator<EligibilityAgencyWorker> validator)
    : BasePageModel
{
    [BindProperty] public bool? IsAgencyWorker { get; set; }

    public PageResult OnGet()
    {
        BackLinkPath = linkGenerator.EligibilityStatutoryWork();
        IsAgencyWorker = createUserJourneyService.GetIsAgencyWorker();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var validationResult = await validator.ValidateAsync(this);
        if (IsAgencyWorker is null || !validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            BackLinkPath = linkGenerator.EligibilityStatutoryWork();
            return Page();
        }

        createUserJourneyService.SetIsAgencyWorker(IsAgencyWorker);

        return Redirect(IsAgencyWorker is false
            ? linkGenerator.EligibilityQualification()
            : linkGenerator.EligibilityFundingNotAvailable());
    }
}
