using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using GovUk.Frontend.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageOrganisations;

[AuthorizeRoles(RoleType.Administrator)]
public class CheckYourAnswers(
    ICreateOrganisationJourneyService createOrganisationJourneyService,
    EcfLinkGenerator linkGenerator
) : BasePageModel
{
    [BindProperty]
    public Organisation? Organisation { get; set; }

    [BindProperty]
    public AccountDetails? PrimaryCoordinator { get; set; }

    public string? ChangeLocalAuthorityCodeLink { get; set; }

    public PageResult OnGet()
    {
        BackLinkPath = linkGenerator.ManageOrganisations.AddPrimaryCoordinator();
        Organisation = createOrganisationJourneyService.GetOrganisation();
        PrimaryCoordinator = createOrganisationJourneyService.GetPrimaryCoordinatorAccountDetails();
        ChangeLocalAuthorityCodeLink = linkGenerator.ManageOrganisations.EnterLocalAuthorityCodeChange();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Organisation is null || PrimaryCoordinator is null)
            return BadRequest();

        await createOrganisationJourneyService.CompleteJourneyAsync();

        TempData["NotificationType"] = NotificationBannerType.Success;
        TempData["NotificationHeader"] = $"{Organisation.OrganisationName} has been added";
        TempData["NotificationMessage"] = $"An invitation email has been sent to {PrimaryCoordinator.FullName}, {PrimaryCoordinator.Email}";

        return Redirect(linkGenerator.ManageOrganisations.Index());
    }
}
