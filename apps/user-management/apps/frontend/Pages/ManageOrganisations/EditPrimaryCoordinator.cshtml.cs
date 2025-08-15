using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageOrganisations;

public class EditPrimaryCoordinator(
    IEditOrganisationJourneyService editOrganisationJourneyService,
    EcfLinkGenerator linkGenerator,
    IValidator<AccountDetails> validator)
    : BasePageModel
{
    [BindProperty] public AccountDetails PrimaryCoordinator { get; set; } = null!;

    [BindProperty] public string? OrganisationName { get; set; }

    public bool IsReplace { get; set; }

    public async Task<PageResult> OnGetAsync(Guid id)
    {
        BackLinkPath = linkGenerator.ManageOrganisations.EditPrimaryCoordinatorChangeType(id);

        var organisation = await editOrganisationJourneyService.GetOrganisationAsync(id);
        if (organisation is not null)
            OrganisationName = organisation.OrganisationName;

        var primaryCoordinator = await editOrganisationJourneyService.GetPrimaryCoordinatorAccountAsync(id);
        if (primaryCoordinator is not null)
            PrimaryCoordinator = primaryCoordinator;

        return Page();
    }

    public async Task<PageResult> OnGetReplaceAsync(Guid id)
    {
        IsReplace = true;
        BackLinkPath = linkGenerator.ManageOrganisations.EditPrimaryCoordinatorChangeType(id);

        var organisation = await editOrganisationJourneyService.GetOrganisationAsync(id);
        if (organisation is not null)
            OrganisationName = organisation.OrganisationName;

        return Page();
    }

    public async Task<PageResult> OnGetReplaceChangeAsync(Guid id)
    {
        IsReplace = true;
        return await OnGetAsync(id);
    }

    public async Task<IActionResult> OnPostAsync(Guid id)
    {
        PrimaryCoordinator.PhoneNumberRequired = true;

        var validationResult = await validator.ValidateAsync(PrimaryCoordinator);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState, nameof(PrimaryCoordinator));
            BackLinkPath = linkGenerator.ManageOrganisations.EditPrimaryCoordinatorChangeType(id);
            OrganisationName = (await editOrganisationJourneyService.GetOrganisationAsync(id))?.OrganisationName;
            return Page();
        }

        await editOrganisationJourneyService.SetPrimaryCoordinatorAccountAsync(id, PrimaryCoordinator);

        return Redirect(
            IsReplace
                ? linkGenerator.ManageOrganisations.CheckYourAnswersReplace(id)
                : linkGenerator.ManageOrganisations.CheckYourAnswersEdit(id));
    }

    public async Task<IActionResult> OnPostReplaceAsync(Guid id)
    {
        IsReplace = true;
        return await OnPostAsync(id);
    }

    public async Task<IActionResult> OnPostReplaceChangeAsync(Guid id)
    {
        IsReplace = true;
        return await OnPostAsync(id);
    }
}
