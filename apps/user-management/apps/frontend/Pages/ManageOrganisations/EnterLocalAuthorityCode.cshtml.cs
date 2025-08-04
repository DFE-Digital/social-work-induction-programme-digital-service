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
        var result = await validator.ValidateAsync(this);
        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            BackLinkPath = linkGenerator.ManageOrganisations.Index();
            return Page();
        }

        createOrganisationJourneyService.SetLocalAuthorityCode(LocalAuthorityCode);

        var organisation = organisationService.GetByLocalAuthorityCode(LocalAuthorityCode);
        // TODO show error if organisation not found once validation and error message designs are available
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
