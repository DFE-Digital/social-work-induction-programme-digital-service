using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.SocialWorkerRegistration;

[AuthorizeRoles(RoleType.EarlyCareerSocialWorker)]
public class SelectSocialWorkQualificationEndYear(
    EcfLinkGenerator linkGenerator,
    IRegisterSocialWorkerJourneyService socialWorkerJourneyService,
    IAuthServiceClient authServiceClient,
    IValidator<SelectSocialWorkQualificationEndYear> validator)
    : BasePageModel
{
    [BindProperty] public int? SocialWorkQualificationEndYear { get; set; }

    public async Task<PageResult> OnGetAsync()
    {
        var personId = authServiceClient.HttpContextService.GetPersonId();
        SocialWorkQualificationEndYear = await socialWorkerJourneyService.GetSocialWorkQualificationEndYearAsync(personId);

        BackLinkPath = linkGenerator.SocialWorkerRegistrationSelectHighestQualification();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var result = await validator.ValidateAsync(this);
        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
        }

        if (!result.IsValid)
        {
            BackLinkPath = linkGenerator.SocialWorkerRegistrationSelectHighestQualification();
            return Page();
        }

        var personId = authServiceClient.HttpContextService.GetPersonId();
        await socialWorkerJourneyService.SetSocialWorkQualificationEndYearAsync(personId, SocialWorkQualificationEndYear);

        return Redirect(FromChangeLink
            ? linkGenerator.SocialWorkerRegistrationCheckYourAnswers()
            : linkGenerator.SocialWorkerRegistrationSelectRouteIntoSocialWork());
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
