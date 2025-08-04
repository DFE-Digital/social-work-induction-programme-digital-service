using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageOrganisations;

public class OrganisationDetails(
    IEditOrganisationJourneyService editOrganisationJourneyService,
    EcfLinkGenerator linkGenerator
) : BasePageModel
{
    [BindProperty]
    public Organisation? Organisation { get; set; }

    [BindProperty]
    public AccountDetails? PrimaryCoordinator { get; set; }

    public RedirectResult OnGetNew(Guid id)
    {
        editOrganisationJourneyService.ResetEditOrganisationJourneyModel(id);
        return Redirect(linkGenerator.ManageOrganisations.ViewOrganisationDetails(id));
    }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var organisation = await editOrganisationJourneyService.GetOrganisationAsync(id);
        var primaryCoordinator = await editOrganisationJourneyService.GetPrimaryCoordinatorAccountAsync(id);

        if (organisation is null || primaryCoordinator is null)
            return NotFound();

        Organisation = organisation;
        PrimaryCoordinator = primaryCoordinator;

        BackLinkPath = linkGenerator.ManageOrganisations.Index();
        await editOrganisationJourneyService.SetOrganisationAsync(id, Organisation);
        await editOrganisationJourneyService.SetPrimaryCoordinatorAccountAsync(id, PrimaryCoordinator);
        return Page();
    }
}
