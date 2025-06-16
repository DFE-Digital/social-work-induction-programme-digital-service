using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;
using Dfe.Sww.Ecf.Frontend.Models;
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
        SelectedEthnicGroupBlack = await socialWorkerJourneyService.EthnicGroupService.GetEthnicGroupBlackAsync(personId);
        OtherEthnicGroupBlack = await socialWorkerJourneyService.EthnicGroupService.GetOtherEthnicGroupBlackAsync(personId);

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
        await socialWorkerJourneyService.EthnicGroupService.SetEthnicGroupBlackAsync(personId, SelectedEthnicGroupBlack);
        await socialWorkerJourneyService.EthnicGroupService.SetOtherEthnicGroupBlackAsync(personId, OtherEthnicGroupBlack);

        return Redirect(linkGenerator.SocialWorkerRegistrationDateOfBirth()); // TODO update this ECSW disability page
    }
}
