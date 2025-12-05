using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageOrganisations;

public class EnterPhoneNumber(
    ICreateOrganisationJourneyService createOrganisationJourneyService,
    IEditOrganisationJourneyService editOrganisationJourneyService,
    EcfLinkGenerator linkGenerator,
    IValidator<EnterPhoneNumber> validator
) : BasePageModel
{
    [BindProperty] public string? PhoneNumber { get; set; }

    [BindProperty] public Guid? Id { get; set; }

    public async Task<PageResult> OnGetAsync(Guid? id = null)
    {
        if (id.HasValue)
        {
            var organisation = await editOrganisationJourneyService.GetOrganisationAsync(id.Value);
            PhoneNumber = organisation?.PhoneNumber;
            Id = id;
        }
        else
        {
            PhoneNumber = createOrganisationJourneyService.GetOrganisation()?.PhoneNumber;
        }

        SetBackLinkPath();
        return Page();
    }

    public async Task<PageResult> OnGetChangeAsync()
    {
        FromChangeLink = true;
        SetBackLinkPath();
        return await OnGetAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var result = await validator.ValidateAsync(this);
        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            SetBackLinkPath();
            return Page();
        }

        if (Id.HasValue)
        {
            var editedOrganisation = await editOrganisationJourneyService.GetOrganisationAsync(Id.Value);
            if (editedOrganisation is null) throw new InvalidOperationException("Organisation must be set before accessing this page.");
            editedOrganisation.PhoneNumber = PhoneNumber;
            await editOrganisationJourneyService.SetOrganisationAsync(Id.Value, editedOrganisation);
            await editOrganisationJourneyService.SetIsOrganisationUpdateAsync(Id.Value, true);
            return Redirect(linkGenerator.ManageOrganisations.CheckYourAnswersEditPhoneNumber(Id.Value));
        }

        var organisation = createOrganisationJourneyService.GetOrganisation();
        if (organisation is null) throw new InvalidOperationException("Organisation must be set before accessing this page.");
        organisation.PhoneNumber = PhoneNumber;
        createOrganisationJourneyService.SetOrganisation(organisation);

        return Redirect(
            FromChangeLink
                ? linkGenerator.ManageOrganisations.CheckYourAnswersPhoneNumberChange()
                : linkGenerator.ManageOrganisations.AddPrimaryCoordinator()
        );
    }

    public async Task<IActionResult> OnPostChangeAsync()
    {
        FromChangeLink = true;
        SetBackLinkPath();
        return await OnPostAsync();
    }

    private void SetBackLinkPath()
    {
        if (Id.HasValue)
            BackLinkPath = linkGenerator.ManageOrganisations.ViewOrganisationDetails(Id.Value);
        else if (FromChangeLink)
            BackLinkPath = linkGenerator.ManageOrganisations.CheckYourAnswers();
        else
            BackLinkPath = linkGenerator.ManageOrganisations.ConfirmOrganisationDetails();


    }
}
