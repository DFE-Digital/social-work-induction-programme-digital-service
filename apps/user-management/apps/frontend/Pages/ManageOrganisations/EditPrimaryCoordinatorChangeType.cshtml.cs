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
[AuthorizeRoles(RoleType.Coordinator)]
public class EditPrimaryCoordinatorChangeType(
    IEditOrganisationJourneyService editOrganisationJourneyService,
    EcfLinkGenerator linkGenerator,
    IValidator<EditPrimaryCoordinatorChangeType> validator)
    : BasePageModel
{
    [BindProperty] public PrimaryCoordinatorChangeType? ChangeType { get; set; }

    public PageResult OnGet()
    {
        BackLinkPath = linkGenerator.ManageOrganisations.Index(); // TODO update this to org details page
        ChangeType = editOrganisationJourneyService.GetPrimaryCoordinatorChangeType();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var validationResult = await validator.ValidateAsync(this);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            linkGenerator.ManageOrganisations.Index(); // TODO update this to org details page
            return Page();
        }

        editOrganisationJourneyService.SetPrimaryCoordinatorChangeType(ChangeType);

        return Redirect(linkGenerator.ManageOrganisations.Index()); // TODO update this to org details page
    }
}
