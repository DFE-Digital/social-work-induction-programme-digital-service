using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

public class EligibilitySocialWorkEngland(
    ICreateAccountJourneyService createAccountJourneyService,
    IAuthServiceClient authServiceClient,
    EcfLinkGenerator linkGenerator,
    IValidator<EligibilitySocialWorkEngland> validator)
    : ManageAccountsBasePageModel
{
    [BindProperty] public bool? IsRegisteredWithSocialWorkEngland { get; set; }
    [BindProperty] public string? SocialWorkerNumber { get; set; }

    public PageResult OnGet()
    {
        BackLinkPath = FromChangeLink ? linkGenerator.ManageAccount.ConfirmAccountDetails(OrganisationId) : linkGenerator.ManageAccount.EligibilityInformation(OrganisationId);
        IsRegisteredWithSocialWorkEngland = createAccountJourneyService.GetIsRegisteredWithSocialWorkEngland();
        SocialWorkerNumber = createAccountJourneyService.GetAccountDetails()?.SocialWorkEnglandNumber;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var validationResult = await validator.ValidateAsync(this);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            BackLinkPath = linkGenerator.ManageAccount.EligibilityInformation(OrganisationId);
            return Page();
        }

        createAccountJourneyService.SetIsRegisteredWithSocialWorkEngland(IsRegisteredWithSocialWorkEngland);

        if (IsRegisteredWithSocialWorkEngland is false)
        {
            return Redirect(FromChangeLink
                ? linkGenerator.ManageAccount.EligibilitySocialWorkEnglandDropoutChange(OrganisationId)
                : linkGenerator.ManageAccount.EligibilitySocialWorkEnglandDropout(OrganisationId));
        }

        // Validated above but compiler complains if we don't check
        if (string.IsNullOrWhiteSpace(SocialWorkerNumber))
            return BadRequest();

        var accountDetails = createAccountJourneyService.GetAccountDetails() ?? new AccountDetails();
        accountDetails.SocialWorkEnglandNumber = SocialWorkerNumber;
        createAccountJourneyService.SetAccountDetails(accountDetails);

        var isEnrolledInAsye = await authServiceClient.AsyeSocialWorker.ExistsAsync(SocialWorkerNumber);
        createAccountJourneyService.SetIsEnrolledInAsye(isEnrolledInAsye);
        if (isEnrolledInAsye)
        {
            return Redirect(FromChangeLink
                ? linkGenerator.ManageAccount.EligibilitySocialWorkEnglandAsyeDropoutChange()
                : linkGenerator.ManageAccount.EligibilitySocialWorkEnglandAsyeDropout());
        }

        return Redirect(FromChangeLink
            ? linkGenerator.ManageAccount.EligibilityFundingAvailable(OrganisationId)
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
