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

namespace Dfe.Sww.Ecf.Frontend.Pages.SocialWorkerRegistration;

[AuthorizeRoles(RoleType.EarlyCareerSocialWorker)]
public class SelectRouteIntoSocialWork(
    EcfLinkGenerator linkGenerator,
    IRegisterSocialWorkerJourneyService socialWorkerJourneyService,
    IAuthServiceClient authServiceClient,
    IValidator<SelectRouteIntoSocialWork> validator)
    : BasePageModel
{
    [BindProperty] public RouteIntoSocialWork? SelectedRouteIntoSocialWork { get; set; }
    [BindProperty] public string? OtherRouteIntoSocialWork { get; set; }

    public async Task<PageResult> OnGetAsync()
    {
        var personId = authServiceClient.HttpContextService.GetPersonId();
        SelectedRouteIntoSocialWork = await socialWorkerJourneyService.GetRouteIntoSocialWorkAsync(personId);
        OtherRouteIntoSocialWork = await socialWorkerJourneyService.GetOtherRouteIntoSocialWorkAsync(personId);

        BackLinkPath = linkGenerator.SocialWorkerRegistrationSelectSocialWorkQualificationEndYear();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var result = await validator.ValidateAsync(this);
        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            BackLinkPath = linkGenerator.SocialWorkerRegistrationSelectSocialWorkQualificationEndYear();
            return Page();
        }

        var personId = authServiceClient.HttpContextService.GetPersonId();
        await socialWorkerJourneyService.SetRouteIntoSocialWorkAsync(personId, SelectedRouteIntoSocialWork);
        await socialWorkerJourneyService.SetOtherRouteIntoSocialWorkAsync(personId, OtherRouteIntoSocialWork);

        return Redirect(linkGenerator.SocialWorkerRegistrationCheckYourAnswers());
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
