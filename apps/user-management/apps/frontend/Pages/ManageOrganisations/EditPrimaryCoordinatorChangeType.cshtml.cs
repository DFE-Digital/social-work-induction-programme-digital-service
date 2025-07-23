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

/// <summary>
/// Edit Primary Coordinator Change Type
/// </summary>
[AuthorizeRoles(RoleType.Administrator)]
public class EditPrimaryCoordinatorChangeType(
    IEditOrganisationJourneyService editOrganisationJourneyService,
    EcfLinkGenerator linkGenerator,
    IValidator<EditPrimaryCoordinatorChangeType> validator)
    : BasePageModel
{
    [BindProperty] public PrimaryCoordinatorChangeType? ChangeType { get; set; }
    [BindProperty] public string? OrganisationName { get; set; }

    public PageResult OnGet(Guid id)
    {
        BackLinkPath = linkGenerator.ManageOrganisations.ViewOrganisationDetails(id);
        ChangeType = editOrganisationJourneyService.GetPrimaryCoordinatorChangeType();
        var organisation = editOrganisationJourneyService.GetOrganisation();
        OrganisationName = organisation?.OrganisationName;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(Guid id)
    {
        var validationResult = await validator.ValidateAsync(this);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            BackLinkPath = linkGenerator.ManageOrganisations.ViewOrganisationDetails(id);
            OrganisationName = editOrganisationJourneyService.GetOrganisation()?.OrganisationName;
            return Page();
        }

        editOrganisationJourneyService.SetPrimaryCoordinatorChangeType(ChangeType);

        // TODO logic on add primary coordinator for updating user details and for adding a new set of details as part of the edit journey if using the same page
        return Redirect(ChangeType == PrimaryCoordinatorChangeType.ReplaceWithNewCoordinator
            ? linkGenerator.ManageOrganisations.AddPrimaryCoordinatorReplace()
            : linkGenerator.ManageOrganisations.AddPrimaryCoordinatorEdit());
    }
}
