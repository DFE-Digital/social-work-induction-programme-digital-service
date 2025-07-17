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

[AuthorizeRoles(RoleType.Administrator)]
public class EnterLocalAuthorityCode(
    ICreateOrganisationJourneyService createOrganisationJourneyService,
    IOrganisationService organisationService,
    EcfLinkGenerator linkGenerator,
    IValidator<EnterLocalAuthorityCode> validator
    ) : BasePageModel
{
    [BindProperty]
    public int? LocalAuthorityCode { get; set; }

    public PageResult OnGet()
    {
        var localAuthorityCode = createOrganisationJourneyService.GetLocalAuthorityCode();
        if (localAuthorityCode is not null)
        {
            LocalAuthorityCode = localAuthorityCode;
        }

        BackLinkPath = linkGenerator.ManageOrganisations.Index();
        return Page();
    }

    public RedirectResult OnGetNew()
    {
        createOrganisationJourneyService.ResetCreateOrganisationJourneyModel();
        return Redirect(linkGenerator.ManageOrganisations.EnterLocalAuthorityCode());
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

        return Redirect(linkGenerator.ManageOrganisations.ConfirmOrganisationDetails());
    }
}
