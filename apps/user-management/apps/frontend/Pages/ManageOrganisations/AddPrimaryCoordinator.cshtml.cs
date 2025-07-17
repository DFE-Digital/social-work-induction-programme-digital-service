using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageOrganisations;

[AuthorizeRoles(RoleType.Administrator)]
public class AddPrimaryCoordinator(
    ICreateOrganisationJourneyService createOrganisationJourneyService,
    IValidator<AccountDetails> validator,
    EcfLinkGenerator linkGenerator
) : BasePageModel
{
    [BindProperty]
    public AccountDetails AccountDetails { get; set; } = null!;

    public PageResult OnGet()
    {
        AccountDetails = createOrganisationJourneyService.GetPrimaryCoordinatorAccountDetails() ?? new AccountDetails();

        BackLinkPath = linkGenerator.ManageOrganisations.ConfirmOrganisationDetails();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        AccountDetails.PhoneNumberRequired = true;
        var result = await validator.ValidateAsync(AccountDetails);
        if (!result.IsValid)
        {
            result.AddToModelState(ModelState, nameof(AccountDetails));
            BackLinkPath = linkGenerator.ManageOrganisations.EnterLocalAuthorityCode();
            return Page();
        }

        createOrganisationJourneyService.SetPrimaryCoordinatorAccountDetails(AccountDetails);

        // TODO replace with confirm details including coordinator if using a different page to confirm details for organisation only
        return Redirect(linkGenerator.ManageOrganisations.ConfirmOrganisationDetails());
    }


}
