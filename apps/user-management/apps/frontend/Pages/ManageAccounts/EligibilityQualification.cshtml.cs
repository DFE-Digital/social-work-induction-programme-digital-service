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
/// Eligibility Qualification View Model
/// </summary>
[AuthorizeRoles(RoleType.Coordinator)]
public class EligibilityQualification(
    ICreateAccountJourneyService createAccountJourneyService,
    EcfLinkGenerator linkGenerator,
    IValidator<EligibilityQualification> validator)
    : BasePageModel
{
    [BindProperty] public bool? IsQualifiedWithin3Years { get; set; }

    public PageResult OnGet()
    {
        BackLinkPath = linkGenerator.EligibilityAgencyWorker();
        IsQualifiedWithin3Years = createAccountJourneyService.GetIsQualifiedWithin3Years();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var validationResult = await validator.ValidateAsync(this);
        if (IsQualifiedWithin3Years is null || !validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            BackLinkPath = linkGenerator.EligibilityAgencyWorker();
            return Page();
        }

        createAccountJourneyService.SetIsQualifiedWithin3Years(IsQualifiedWithin3Years);

        return Redirect(IsQualifiedWithin3Years is false
            ? linkGenerator.EligibilityFundingNotAvailable()
            : linkGenerator.EligibilityFundingAvailable());
    }
}
