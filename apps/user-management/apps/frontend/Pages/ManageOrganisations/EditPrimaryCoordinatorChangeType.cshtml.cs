using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageOrganisations;

public class EditPrimaryCoordinatorChangeType(
    IEditOrganisationJourneyService editOrganisationJourneyService,
    EcfLinkGenerator linkGenerator,
    IValidator<EditPrimaryCoordinatorChangeType> validator)
    : BasePageModel
{
    [BindProperty] public PrimaryCoordinatorChangeType? ChangeType { get; set; }
    [BindProperty] public string? OrganisationName { get; set; }

    public async Task<PageResult> OnGetAsync(Guid id)
    {
        BackLinkPath = FromChangeLink
            ? linkGenerator.ManageOrganisations.CheckYourAnswersEditChange(id)
            : linkGenerator.ManageOrganisations.ViewOrganisationDetails(id);
        ChangeType = await editOrganisationJourneyService.GetPrimaryCoordinatorChangeTypeAsync(id);
        var organisation = await editOrganisationJourneyService.GetOrganisationAsync(id);
        if (organisation is not null) OrganisationName = organisation.OrganisationName;
        return Page();
    }

    public async Task<PageResult> OnGetChangeAsync(Guid id)
    {
        FromChangeLink = true;
        return await OnGetAsync(id);
    }

    public async Task<IActionResult> OnPostAsync(Guid id)
    {
        var validationResult = await validator.ValidateAsync(this);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            BackLinkPath = linkGenerator.ManageOrganisations.ViewOrganisationDetails(id);
            OrganisationName = (await editOrganisationJourneyService.GetOrganisationAsync(id))?.OrganisationName;
            return Page();
        }

        await editOrganisationJourneyService.SetPrimaryCoordinatorChangeTypeAsync(id, ChangeType);

        if (FromChangeLink)
        {
            return Redirect(ChangeType == PrimaryCoordinatorChangeType.ReplaceWithNewCoordinator
                ? linkGenerator.ManageOrganisations.ReplacePrimaryCoordinatorEditChange(id)
                : linkGenerator.ManageOrganisations.EditPrimaryCoordinatorEditChange(id));
        }

        return Redirect(ChangeType == PrimaryCoordinatorChangeType.ReplaceWithNewCoordinator
            ? linkGenerator.ManageOrganisations.ReplacePrimaryCoordinator(id)
            : linkGenerator.ManageOrganisations.EditPrimaryCoordinator(id));
    }

    public async Task<IActionResult> OnPostChangeAsync(Guid id)
    {
        FromChangeLink = true;
        return await OnPostAsync(id);
    }
}
