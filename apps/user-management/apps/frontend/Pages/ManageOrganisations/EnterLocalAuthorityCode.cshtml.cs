using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Pagination;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageOrganisations;

public class EnterLocalAuthorityCode(
    ICreateOrganisationJourneyService createOrganisationJourneyService,
    IOrganisationService organisationService,
    EcfLinkGenerator linkGenerator,
    IValidator<EnterLocalAuthorityCode> validator
) : BasePageModel
{
    [BindProperty] public int? LocalAuthorityCode { get; set; }

    public PageResult OnGet()
    {
        var localAuthorityCode = createOrganisationJourneyService.GetLocalAuthorityCode();
        if (localAuthorityCode is not null)
        {
            LocalAuthorityCode = localAuthorityCode;
        }

        SetBackLinkPath();
        return Page();
    }

    public RedirectResult OnGetNew()
    {
        createOrganisationJourneyService.ResetCreateOrganisationJourneyModel();
        return Redirect(linkGenerator.ManageOrganisations.EnterLocalAuthorityCode());
    }

    public PageResult OnGetChange()
    {
        FromChangeLink = true;
        SetBackLinkPath();
        return OnGet();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Handle model binding error for non-numeric input
        if (ModelState.TryGetValue(nameof(LocalAuthorityCode), out var input) && input.Errors.Count > 0)
        {
            input.Errors.Clear();
            ModelState.AddModelError(nameof(LocalAuthorityCode), "The local authority code must only include numbers");
            BackLinkPath = linkGenerator.ManageOrganisations.Index();
            return Page();
        }

        var result = await validator.ValidateAsync(this);
        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            BackLinkPath = linkGenerator.ManageOrganisations.Index();
            return Page();
        }

        createOrganisationJourneyService.SetLocalAuthorityCode(LocalAuthorityCode);

        // Validated above but compiler complains if we don't check
        if (LocalAuthorityCode is null)
        {
            return BadRequest();
        }

        var organisation = await organisationService.GetByLocalAuthorityCodeAsync(LocalAuthorityCode.Value);
        if (organisation is null)
        {
            ModelState.AddModelError(nameof(LocalAuthorityCode), "The code you have entered is not associated with a local authority");
            BackLinkPath = linkGenerator.ManageOrganisations.Index();
            return Page();
        }

        createOrganisationJourneyService.SetOrganisation(organisation);

        return Redirect(
            FromChangeLink
                ? linkGenerator.ManageOrganisations.CheckYourAnswers()
                : linkGenerator.ManageOrganisations.ConfirmOrganisationDetails()
        );
    }

    public async Task<IActionResult> OnPostChangeAsync()
    {
        FromChangeLink = true;
        SetBackLinkPath();
        return await OnPostAsync();
    }

    private void SetBackLinkPath()
    {
        BackLinkPath ??= FromChangeLink
            ? linkGenerator.ManageOrganisations.CheckYourAnswers()
            : linkGenerator.ManageOrganisations.Index();
    }
}
