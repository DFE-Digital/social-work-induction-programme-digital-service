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

namespace Dfe.Sww.Ecf.Frontend.Pages.SocialWorkerRegistration;

[AuthorizeRoles(RoleType.EarlyCareerSocialWorker)]
public class SelectSexAndGenderIdentity(
    EcfLinkGenerator linkGenerator,
    IRegisterSocialWorkerJourneyService socialWorkerJourneyService,
    IAuthServiceClient authServiceClient,
    IValidator<SelectSexAndGenderIdentity> validator)
    : BasePageModel
{
    [BindProperty] public UserSex? SelectedUserSex { get; set; }
    [BindProperty] public GenderMatchesSexAtBirth? GenderMatchesSexAtBirth { get; set; }

    public async Task<PageResult> OnGetAsync()
    {
        var personId = authServiceClient.HttpContextService.GetPersonId();
        SelectedUserSex = await socialWorkerJourneyService.GetUserSexAsync(personId);
        GenderMatchesSexAtBirth = await socialWorkerJourneyService.GetUserGenderMatchesSexAtBirthAsync(personId);

        BackLinkPath = linkGenerator.SocialWorkerRegistrationDateOfBirth();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var result = await validator.ValidateAsync(this);
        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            BackLinkPath = linkGenerator.SocialWorkerRegistrationDateOfBirth();
            return Page();
        }

        var personId = authServiceClient.HttpContextService.GetPersonId();
        await socialWorkerJourneyService.SetUserSexAsync(personId, SelectedUserSex);
        await socialWorkerJourneyService.SetUserGenderMatchesSexAtBirthAsync(personId, GenderMatchesSexAtBirth);

        return Redirect(linkGenerator.SocialWorkerRegistrationDateOfBirth()); // TODO update this ECSW select ethnic group page
    }
}
