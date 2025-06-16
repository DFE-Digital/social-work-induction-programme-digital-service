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
public class Index(
    EcfLinkGenerator linkGenerator,
    IRegisterSocialWorkerJourneyService socialWorkerJourneyService,
    IAuthServiceClient authServiceClient,
    IValidator<Index> validator)
    : BasePageModel
{
    [BindProperty] public EthnicGroup? SelectedEthnicGroup { get; set; }

    public async Task<PageResult> OnGetAsync()
    {
        var personId = authServiceClient.HttpContextService.GetPersonId();
        SelectedEthnicGroup = await socialWorkerJourneyService.EthnicGroupService.GetEthnicGroupAsync(personId);

        BackLinkPath = linkGenerator.SocialWorkerRegistrationSexAndGenderIdentity();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var result = await validator.ValidateAsync(this);
        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            BackLinkPath = linkGenerator.SocialWorkerRegistrationSexAndGenderIdentity();
            return Page();
        }

        var personId = authServiceClient.HttpContextService.GetPersonId();
        await socialWorkerJourneyService.EthnicGroupService.SetEthnicGroupAsync(personId, SelectedEthnicGroup);

        return SelectedEthnicGroup switch
        {
            EthnicGroup.White => Redirect(linkGenerator.SocialWorkerRegistrationEthnicGroupWhite()),
            EthnicGroup.MixedOrMultipleEthnicGroups => Redirect(linkGenerator.SocialWorkerRegistrationEthnicGroupMixed()),
            EthnicGroup.AsianOrAsianBritish => Redirect(linkGenerator.SocialWorkerRegistrationEthnicGroupAsian()),
            EthnicGroup.BlackAfricanCaribbeanOrBlackBritish => Redirect(linkGenerator.SocialWorkerRegistrationEthnicGroupBlack()),
            EthnicGroup.OtherEthnicGroup => Redirect(linkGenerator.SocialWorkerRegistrationEthnicGroupOther()),
            EthnicGroup.PreferNotToSay => Redirect(linkGenerator.SocialWorkerRegistrationDateOfBirth()), // TODO update this ECSW disability page
            _ => Page()
        };
    }
}
