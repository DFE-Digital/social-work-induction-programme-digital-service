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
    IManageOrganisationJourneyService manageOrganisationJourneyService,
    EcfLinkGenerator linkGenerator
) : BasePageModel
{
    [BindProperty]
    public Organisation? Organisation { get; set; }

    [BindProperty]
    public AccountDetails? PrimaryCoordinator { get; set; }

    public string? ChangeLocalAuthorityCodeLink { get; set; }
    public string? ChangePrimaryCoordinatorLink { get; set; }

    public PageResult OnGet()
    {
        BackLinkPath = linkGenerator.ManageOrganisations.AddPrimaryCoordinator();
        Organisation = manageOrganisationJourneyService.GetOrganisation();
        PrimaryCoordinator = manageOrganisationJourneyService.GetPrimaryCoordinatorAccountDetails();
        ChangeLocalAuthorityCodeLink = linkGenerator.ManageOrganisations.EnterLocalAuthorityCodeChange();
        ChangePrimaryCoordinatorLink = linkGenerator.ManageOrganisations.AddPrimaryCoordinatorChange();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var organisation = manageOrganisationJourneyService.GetOrganisation();
        var primaryCoordinator = manageOrganisationJourneyService.GetPrimaryCoordinatorAccountDetails();
        if (organisation is null || primaryCoordinator is null)
            return BadRequest();

        await manageOrganisationJourneyService.CompleteJourneyAsync();

        TempData["NotificationType"] = NotificationBannerType.Success;
        TempData["NotificationHeader"] = $"{organisation.OrganisationName} has been added";
        TempData["NotificationMessage"] = $"An invitation email has been sent to {primaryCoordinator.FullName}, {primaryCoordinator.Email}";

        return Redirect(linkGenerator.ManageOrganisations.Index());
    }
}
