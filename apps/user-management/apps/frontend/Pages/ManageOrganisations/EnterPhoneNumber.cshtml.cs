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
    EcfLinkGenerator linkGenerator,
    IValidator<EnterPhoneNumber> validator
) : BasePageModel
{
    [BindProperty] public string? PhoneNumber { get; set; }

    public PageResult OnGet()
    {
        var phoneNumber = createOrganisationJourneyService.GetPhoneNumber();
        if (phoneNumber is not null)
        {
            PhoneNumber = phoneNumber;
        }

        SetBackLinkPath();
        return Page();
    }

    public PageResult OnGetChange()
    {
        FromChangeLink = true;
        SetBackLinkPath();
        return OnGet();
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

        createOrganisationJourneyService.SetPhoneNumber(PhoneNumber);

        return Redirect(
            FromChangeLink
                ? linkGenerator.ManageOrganisations.CheckYourAnswers()
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
        BackLinkPath ??= FromChangeLink
            ? linkGenerator.ManageOrganisations.CheckYourAnswers()
            : linkGenerator.ManageOrganisations.ConfirmOrganisationDetails();
    }
}
