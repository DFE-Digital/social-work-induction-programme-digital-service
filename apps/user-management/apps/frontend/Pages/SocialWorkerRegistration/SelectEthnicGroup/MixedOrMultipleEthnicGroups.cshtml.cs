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
public class SelectMixedEthnicGroup(
    EcfLinkGenerator linkGenerator,
    IRegisterSocialWorkerJourneyService socialWorkerJourneyService,
    IAuthServiceClient authServiceClient,
    IValidator<SelectMixedEthnicGroup> validator)
    : BasePageModel
{
    [BindProperty] public EthnicGroupMixed? SelectedEthnicGroupMixed { get; set; }
    [BindProperty] public string? OtherMixedEthnicGroup { get; set; }

    public async Task<PageResult> OnGetAsync()
    {
        var personId = authServiceClient.HttpContextService.GetPersonId();
        SelectedEthnicGroupMixed = await socialWorkerJourneyService.GetMixedEthnicGroupAsync(personId);
        OtherMixedEthnicGroup = await socialWorkerJourneyService.GetOtherMixedEthnicGroupAsync(personId);

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
        await socialWorkerJourneyService.SetEthnicGroupMixedAsync(personId, SelectedEthnicGroupMixed);
        await socialWorkerJourneyService.SetOtherMixedEthnicGroupAsync(personId, OtherMixedEthnicGroup);

        return Redirect(linkGenerator.SocialWorkerRegistrationDateOfBirth()); // TODO update this ECSW disability page
    }
}
