using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using GovUk.Frontend.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageOrganisations;

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
    public string? ChangePhoneNumberLink { get; set; }
    public bool IsEdit { get; set; }
    public bool IsEditPhoneNumber { get; set; }
    public bool IsReplace { get; set; }
    public bool IsFromPhoneNumberChange { get; set; }
    public bool IsFromLocalAuthorityChange { get; set; }

    public PageResult OnGet()
    {
        BackLinkPath = GetBackLink();
        Organisation = createOrganisationJourneyService.GetOrganisation();
        PrimaryCoordinator = createOrganisationJourneyService.GetPrimaryCoordinatorAccountDetails();
        ChangeLocalAuthorityCodeLink = linkGenerator.ManageOrganisations.EnterLocalAuthorityCodeChange();
        ChangePrimaryCoordinatorLink = linkGenerator.ManageOrganisations.AddPrimaryCoordinatorChange();
        ChangePhoneNumberLink = linkGenerator.ManageOrganisations.EnterPhoneNumberChange();

        return Page();
    }

    public PageResult OnGetFromPhoneNumberChange()
    {
        IsFromPhoneNumberChange = true;
        return OnGet();
    }

    public PageResult OnGetFromLocalAuthorityChange()
    {
        IsFromLocalAuthorityChange = true;
        return OnGet();
    }

    public async Task<PageResult> OnGetEditAsync(Guid id)
    {
        IsEdit = true;
        await GetEditReplaceDataAsync(id);

        return Page();
    }

    public async Task<PageResult> OnGetReplaceAsync(Guid id)
    {
        IsReplace = true;
        await GetEditReplaceDataAsync(id);

        return Page();
    }

    public async Task<PageResult> OnGetEditPhoneNumberAsync(Guid id)
    {
        IsEditPhoneNumber = true;
        await GetEditReplaceDataAsync(id);

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

        await editOrganisationJourneyService.CompleteJourneyAsync(id, updateAccount: true);

        TempData["NotificationType"] = NotificationBannerType.Success;
        TempData["NotificationHeader"] = $"{organisation.OrganisationName} has been updated";

        return Redirect(linkGenerator.ManageOrganisations.Index());
    }


    public async Task<IActionResult> OnPostEditPhoneNumberAsync(Guid id)
    {
        var organisation = await editOrganisationJourneyService.GetOrganisationAsync(id);
        if (organisation is null)
            return BadRequest();

        await editOrganisationJourneyService.CompleteJourneyAsync(id, updateOrganisation: true);

        TempData["NotificationType"] = NotificationBannerType.Success;
        TempData["NotificationHeader"] = $"{organisation.OrganisationName} has been updated";

        return Redirect(linkGenerator.ManageOrganisations.Index());
    }

    public async Task<IActionResult> OnPostReplaceAsync(Guid id)
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

    private async Task GetEditReplaceDataAsync(Guid id)
    {
        BackLinkPath = GetBackLink(id);
        Organisation = await editOrganisationJourneyService.GetOrganisationAsync(id);
        PrimaryCoordinator = await editOrganisationJourneyService.GetPrimaryCoordinatorAccountAsync(id);
        ChangeLocalAuthorityCodeLink = null;
        ChangePhoneNumberLink = linkGenerator.ManageOrganisations.EnterPhoneNumberEdit(id);
        ChangePrimaryCoordinatorLink = IsReplace
            ? linkGenerator.ManageOrganisations.ReplacePrimaryCoordinatorChange(id)
            : linkGenerator.ManageOrganisations.EditPrimaryCoordinator(id);
    }

    public string GetBackLink(Guid? id = null)
    {
        if (IsEdit && id.HasValue)
            return linkGenerator.ManageOrganisations.EditPrimaryCoordinator(id.Value);

        if (IsEditPhoneNumber && id.HasValue)
            return linkGenerator.ManageOrganisations.EditPrimaryCoordinator(id.Value);

        if (IsReplace && id.HasValue)
            return linkGenerator.ManageOrganisations.ReplacePrimaryCoordinatorChange(id.Value);

        if (IsFromPhoneNumberChange)
            return linkGenerator.ManageOrganisations.EnterPhoneNumberChange();

        if (IsFromLocalAuthorityChange)
            return linkGenerator.ManageOrganisations.EnterLocalAuthorityCodeChange();

        return linkGenerator.ManageOrganisations.AddPrimaryCoordinator();
    }
}
