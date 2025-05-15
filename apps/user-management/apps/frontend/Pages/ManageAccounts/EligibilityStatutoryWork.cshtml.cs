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
        BackLinkPath = linkGenerator.EligibilitySocialWorkEngland();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var validationResult = await validator.ValidateAsync(this);
        if (IsStatutoryWorker is null || !validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            BackLinkPath = linkGenerator.EligibilityInformation();
            return Page();
        }

        createAccountJourneyService.SetIsStatutoryWorker(IsStatutoryWorker);

        return Redirect(IsStatutoryWorker is false
            ? linkGenerator.EligibilitySocialWorkEnglandDropout() // TODO: Redirect to statutory work dropout page in SWIP-592
            : linkGenerator.AddAccountDetails()); // TODO: Redirect to agency work page in SWIP-580
    }
}
