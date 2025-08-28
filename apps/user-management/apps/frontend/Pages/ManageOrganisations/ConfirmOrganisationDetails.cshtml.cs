using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageOrganisations;

public class ConfirmOrganisationDetails(
    ICreateOrganisationJourneyService createOrganisationJourneyService,
    EcfLinkGenerator linkGenerator
) : BasePageModel
{
    [BindProperty]
    public Organisation? Organisation { get; set; }

    public PageResult OnGet()
    {
        BackLinkPath = linkGenerator.ManageOrganisations.EnterLocalAuthorityCode();
        Organisation = createOrganisationJourneyService.GetOrganisation();

        return Page();
    }

    public IActionResult OnPost()
    {
        return Redirect(linkGenerator.ManageOrganisations.EnterPhoneNumber());
    }
}
