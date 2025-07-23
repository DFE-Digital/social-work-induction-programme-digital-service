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
    IManageOrganisationJourneyService manageOrganisationJourneyService,
    IValidator<AccountDetails> validator,
    EcfLinkGenerator linkGenerator
) : BasePageModel
{
    [BindProperty]
    public AccountDetails AccountDetails { get; set; } = null!;

    public PageResult OnGet()
    {
        AccountDetails = manageOrganisationJourneyService.GetPrimaryCoordinatorAccountDetails() ?? new AccountDetails();
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
        AccountDetails.PhoneNumberRequired = true;
        var result = await validator.ValidateAsync(AccountDetails);
        if (!result.IsValid)
        {
            result.AddToModelState(ModelState, nameof(AccountDetails));
            BackLinkPath = linkGenerator.ManageOrganisations.EnterLocalAuthorityCode();
            return Page();
        }

        AccountDetails.Types = new List<AccountType> { AccountType.Coordinator };
        manageOrganisationJourneyService.SetPrimaryCoordinatorAccountDetails(AccountDetails);

        return Redirect(linkGenerator.ManageOrganisations.CheckYourAnswers());
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
