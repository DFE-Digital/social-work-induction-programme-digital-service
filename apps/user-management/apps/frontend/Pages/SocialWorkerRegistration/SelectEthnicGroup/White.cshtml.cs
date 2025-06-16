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
public class SelectEthnicGroupWhite(
    EcfLinkGenerator linkGenerator,
    IRegisterSocialWorkerJourneyService socialWorkerJourneyService,
    IAuthServiceClient authServiceClient,
    IValidator<SelectEthnicGroupWhite> validator)
    : BasePageModel
{
    [BindProperty] public EthnicGroupWhite? SelectedEthnicGroupWhite { get; set; }
    [BindProperty] public string? OtherEthnicGroupWhite { get; set; }

    public async Task<PageResult> OnGetAsync()
    {
        var personId = authServiceClient.HttpContextService.GetPersonId();
        SelectedEthnicGroupWhite = await socialWorkerJourneyService.EthnicGroupService.GetEthnicGroupWhiteAsync(personId);
        OtherEthnicGroupWhite = await socialWorkerJourneyService.EthnicGroupService.GetOtherEthnicGroupWhiteAsync(personId);

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
        await socialWorkerJourneyService.EthnicGroupService.SetEthnicGroupWhiteAsync(personId, SelectedEthnicGroupWhite);
        await socialWorkerJourneyService.EthnicGroupService.SetOtherEthnicGroupWhiteAsync(personId, OtherEthnicGroupWhite);

        return Redirect(linkGenerator.SocialWorkerRegistrationDateOfBirth()); // TODO update this ECSW disability page
    }
}
