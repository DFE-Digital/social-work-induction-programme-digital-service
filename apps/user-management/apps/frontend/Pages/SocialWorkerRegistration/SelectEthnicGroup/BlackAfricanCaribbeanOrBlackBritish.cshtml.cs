using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;
using Dfe.Sww.Ecf.Frontend.Models.RegisterSocialWorker;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.SocialWorkerRegistration.SelectEthnicGroup;

[AuthorizeRoles(RoleType.EarlyCareerSocialWorker)]
public class SelectEthnicGroupBlack(
    EcfLinkGenerator linkGenerator,
    IRegisterSocialWorkerJourneyService socialWorkerJourneyService,
    IAuthServiceClient authServiceClient,
    IValidator<SelectEthnicGroupBlack> validator)
    : BasePageModel
{
    [BindProperty] public EthnicGroupBlack? SelectedEthnicGroupBlack { get; set; }
    [BindProperty] public string? OtherEthnicGroupBlack { get; set; }

    public async Task<PageResult> OnGetAsync()
    {
        var personId = authServiceClient.HttpContextService.GetPersonId();
        SelectedEthnicGroupBlack = await socialWorkerJourneyService.EthnicGroups.GetEthnicGroupBlackAsync(personId);
        OtherEthnicGroupBlack = await socialWorkerJourneyService.EthnicGroups.GetOtherEthnicGroupBlackAsync(personId);

        BackLinkPath = linkGenerator.SocialWorkerRegistrationEthnicGroup();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var result = await validator.ValidateAsync(this);
        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            BackLinkPath = linkGenerator.SocialWorkerRegistrationEthnicGroup();
            return Page();
        }

        var personId = authServiceClient.HttpContextService.GetPersonId();
        await socialWorkerJourneyService.EthnicGroups.SetEthnicGroupBlackAsync(personId, SelectedEthnicGroupBlack);
        await socialWorkerJourneyService.EthnicGroups.SetOtherEthnicGroupBlackAsync(personId, OtherEthnicGroupBlack);

        return Redirect(FromChangeLink
            ? linkGenerator.SocialWorkerRegistrationCheckYourAnswers()
            : linkGenerator.SocialWorkerRegistrationSelectDisability());
    }

    public Task<PageResult> OnGetChangeAsync()
    {
        FromChangeLink = true;
        return OnGetAsync();
    }

    public async Task<IActionResult> OnPostChangeAsync()
    {
        FromChangeLink = true;
        return await OnPostAsync();
    }
}
