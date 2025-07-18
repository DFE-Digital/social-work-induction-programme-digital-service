using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageOrganisations;

[AuthorizeRoles(RoleType.Administrator)]
public class OrganisationDetails(
    IEditOrganisationJourneyService editOrganisationJourneyService,
    IOrganisationService organisationService,
    EcfLinkGenerator linkGenerator
) : BasePageModel
{
    [BindProperty]
    public Organisation Organisation { get; set; } = null!;

    [BindProperty]
    public AccountDetails PrimaryCoordinator { get; set; } = null!;

    public RedirectResult OnGetNew(Guid id)
    {
        editOrganisationJourneyService.ResetCreateOrganisationJourneyModel();
        return Redirect(linkGenerator.ViewOrganisationDetails(id));
    }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var organisation = await organisationService.GetByIdAsync(id);
        if (organisation is null) return NotFound();

        BackLinkPath = linkGenerator.ManageOrganisations();
        Organisation = organisation;

        // TODO get coordinator

        return Page();
    }
}
