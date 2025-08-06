using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

public class EligibilitySocialWorkEngland(
    ICreateAccountJourneyService createAccountJourneyService,
    EcfLinkGenerator linkGenerator,
    IValidator<EligibilitySocialWorkEngland> validator)
    : ManageAccountsBasePageModel
{
    [BindProperty] public bool? IsRegisteredWithSocialWorkEngland { get; set; }

    public PageResult OnGet()
    {
        BackLinkPath = FromChangeLink ? linkGenerator.ManageAccount.ConfirmAccountDetails(OrganisationId) : linkGenerator.ManageAccount.EligibilityInformation(OrganisationId);
        IsRegisteredWithSocialWorkEngland = createAccountJourneyService.GetIsRegisteredWithSocialWorkEngland();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var validationResult = await validator.ValidateAsync(this);
        if (IsRegisteredWithSocialWorkEngland is null || !validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            BackLinkPath = linkGenerator.ManageAccount.EligibilityInformation(OrganisationId);
            return Page();
        }

        createAccountJourneyService.SetIsRegisteredWithSocialWorkEngland(IsRegisteredWithSocialWorkEngland);
        if (FromChangeLink)
        {
            if (IsRegisteredWithSocialWorkEngland == true)
            {
                return Redirect(linkGenerator.ManageAccount.ConfirmAccountDetails(OrganisationId));
            }
            return Redirect(linkGenerator.ManageAccount.EligibilitySocialWorkEnglandDropoutChange(OrganisationId));
        }
        return Redirect(IsRegisteredWithSocialWorkEngland is false
            ? linkGenerator.ManageAccount.EligibilitySocialWorkEnglandDropout(OrganisationId)
            : linkGenerator.ManageAccount.EligibilityStatutoryWork(OrganisationId));
    }

    public PageResult OnGetChange()
    {
        FromChangeLink = true;
        return OnGet();
    }

    public async Task<IActionResult> OnPostChangeAsync()
    {
        FromChangeLink = true;
        return await OnPostAsync();
    }
}
