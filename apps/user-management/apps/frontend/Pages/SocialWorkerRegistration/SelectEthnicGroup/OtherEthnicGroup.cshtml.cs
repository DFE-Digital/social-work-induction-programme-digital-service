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
public class SelectEthnicGroupOther(
    EcfLinkGenerator linkGenerator,
    IRegisterSocialWorkerJourneyService socialWorkerJourneyService,
    IAuthServiceClient authServiceClient,
    IValidator<SelectEthnicGroupOther> validator)
    : BasePageModel
{
    [BindProperty] public EthnicGroupOther? SelectedEthnicGroupOther { get; set; }
    [BindProperty] public string? OtherEthnicGroupOther { get; set; }

    public async Task<PageResult> OnGetAsync()
    {
        var personId = authServiceClient.HttpContextService.GetPersonId();
        SelectedEthnicGroupOther = await socialWorkerJourneyService.EthnicGroupService.GetEthnicGroupOtherAsync(personId);
        OtherEthnicGroupOther = await socialWorkerJourneyService.EthnicGroupService.GetOtherEthnicGroupOtherAsync(personId);

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
        await socialWorkerJourneyService.EthnicGroupService.SetEthnicGroupOtherAsync(personId, SelectedEthnicGroupOther);
        await socialWorkerJourneyService.EthnicGroupService.SetOtherEthnicGroupOtherAsync(personId, OtherEthnicGroupOther);

        return Redirect(linkGenerator.SocialWorkerRegistrationDateOfBirth()); // TODO update this ECSW disability page
    }
}
