using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageOrganisations;

[AuthorizeRoles(RoleType.Administrator)]
public class OrganisationDetails(
    IEditOrganisationJourneyService editOrganisationJourneyService,
    IOrganisationService organisationService,
    IAccountService accountService,
    EcfLinkGenerator linkGenerator
) : BasePageModel
{
    [BindProperty]
    public Organisation? Organisation { get; set; }

    [BindProperty]
    public Account? PrimaryCoordinator { get; set; }

    public RedirectResult OnGetNew(Guid id)
    {
        editOrganisationJourneyService.ResetCreateOrganisationJourneyModel();
        return Redirect(linkGenerator.ViewOrganisationDetails(id));
    }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var organisation = await organisationService.GetByIdAsync(id);
        if (organisation is null) return NotFound();

        if (organisation.PrimaryCoordinatorId is null) return NotFound();

        var primaryCoordinator = await accountService.GetByIdAsync(organisation.PrimaryCoordinatorId.Value);
        if (primaryCoordinator is null) return NotFound();

        Organisation = organisation;
        PrimaryCoordinator = primaryCoordinator;

        BackLinkPath = linkGenerator.ManageOrganisations();
        return Page();
    }
}
