using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageOrganisations;

[AuthorizeRoles(RoleType.Administrator)]
public class ConfirmOrganisationDetails(
    ICreateOrganisationJourneyService createOrganisationJourneyService,
    EcfLinkGenerator linkGenerator
) : BasePageModel
{
    [BindProperty]
    public Organisation? Organisation { get; set; }

    public PageResult OnGet()
    {
        BackLinkPath = linkGenerator.SocialWorkerProgrammeDates();
        Organisation = createOrganisationJourneyService.GetOrganisation();

        Organisation = new Organisation
        {
            OrganisationId = Guid.NewGuid(),
            OrganisationName = "Test Org",
            ExternalOrganisationId = 123,
            LocalAuthorityCode = 321,
            Type = OrganisationType.LocalAuthority,
            Region = "Yorkshire and The Humber",
        };

        return Page();
    }

    public IActionResult OnPost()
    {
        // TODO Redirect the user to the primary coordinator page here
        return Redirect(linkGenerator.ManageAccounts());
    }
}
