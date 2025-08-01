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
    IEditOrganisationJourneyService editOrganisationJourneyService,
    EcfLinkGenerator linkGenerator
) : BasePageModel
{
    [BindProperty] public Organisation? Organisation { get; set; }

    [BindProperty] public AccountDetails? PrimaryCoordinator { get; set; }

    public string? ChangeLocalAuthorityCodeLink { get; set; }
    public string? ChangePrimaryCoordinatorLink { get; set; }
    public bool IsEdit { get; set; }

    public PageResult OnGet()
    {
        BackLinkPath = linkGenerator.ManageOrganisations.AddPrimaryCoordinator();
        Organisation = createOrganisationJourneyService.GetOrganisation();
        PrimaryCoordinator = createOrganisationJourneyService.GetPrimaryCoordinatorAccountDetails();
        ChangeLocalAuthorityCodeLink = linkGenerator.ManageOrganisations.EnterLocalAuthorityCodeChange();
        ChangePrimaryCoordinatorLink = linkGenerator.ManageOrganisations.AddPrimaryCoordinatorChange();

        return Page();
    }

    public async Task<PageResult> OnGetEditAsync(Guid id)
    {
        IsEdit = true;
        BackLinkPath = linkGenerator.ManageOrganisations.EditPrimaryCoordinator(id);
        Organisation = await editOrganisationJourneyService.GetOrganisationAsync(id);
        PrimaryCoordinator = await editOrganisationJourneyService.GetPrimaryCoordinatorAccountAsync(id);
        ChangeLocalAuthorityCodeLink = null;
        ChangePrimaryCoordinatorLink = linkGenerator.ManageOrganisations.EditPrimaryCoordinator(id);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var organisation = createOrganisationJourneyService.GetOrganisation();
        var primaryCoordinator = createOrganisationJourneyService.GetPrimaryCoordinatorAccountDetails();
        if (organisation is null || primaryCoordinator is null)
            return BadRequest();

        await createOrganisationJourneyService.CompleteJourneyAsync();

        TempData["NotificationType"] = NotificationBannerType.Success;
        TempData["NotificationHeader"] = $"{organisation.OrganisationName} has been added";
        TempData["NotificationMessage"] = $"An invitation email has been sent to {primaryCoordinator.FullName}, {primaryCoordinator.Email}";

        return Redirect(linkGenerator.ManageOrganisations.Index());
    }

    public async Task<IActionResult> OnPostEditAsync(Guid id)
    {
        var organisation = await editOrganisationJourneyService.GetOrganisationAsync(id);
        var primaryCoordinator = await editOrganisationJourneyService.GetPrimaryCoordinatorAccountAsync(id);
        if (organisation is null || primaryCoordinator is null)
            return BadRequest();

        await editOrganisationJourneyService.CompleteJourneyAsync(id);

        TempData["NotificationType"] = NotificationBannerType.Success;
        TempData["NotificationHeader"] = $"{organisation.OrganisationName} has been updated";
        TempData["NotificationMessage"] = $"An invitation email has been sent to {primaryCoordinator.FullName}, {primaryCoordinator.Email}";

        return Redirect(linkGenerator.ManageOrganisations.Index());
    }
}
