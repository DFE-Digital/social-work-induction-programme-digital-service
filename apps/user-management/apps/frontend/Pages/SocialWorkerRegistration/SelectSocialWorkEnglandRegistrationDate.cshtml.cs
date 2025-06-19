using System.ComponentModel.DataAnnotations;
using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Journeys.Interfaces;
using FluentValidation;
using GovUk.Frontend.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NodaTime;

namespace Dfe.Sww.Ecf.Frontend.Pages.SocialWorkerRegistration;

[AuthorizeRoles(RoleType.EarlyCareerSocialWorker)]
public class SelectSocialWorkEnglandRegistrationDate(
    EcfLinkGenerator linkGenerator,
    IRegisterSocialWorkerJourneyService socialWorkerJourneyService,
    IAuthServiceClient authServiceClient,
    IValidator<SelectSocialWorkEnglandRegistrationDate> validator)
    : BasePageModel
{
    [BindProperty]
    [DateInput(DateInputItemTypes.All, ErrorMessagePrefix = "Date you were added to the Social Work England register")]
    [Required(ErrorMessage = "Enter the date you were added to the Social Work England register")]
    public LocalDate? SocialWorkEnglandRegistrationDate { get; set; }

    public async Task<PageResult> OnGetAsync()
    {
        var personId = authServiceClient.HttpContextService.GetPersonId();
        var dob = await socialWorkerJourneyService.GetSocialWorkEnglandRegistrationDateAsync(personId);
        if (dob.HasValue)
        {
            SocialWorkEnglandRegistrationDate = LocalDate.FromDateOnly(dob.Value);
        }

        BackLinkPath = linkGenerator.SocialWorkerRegistrationSelectDisability();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var result = await validator.ValidateAsync(this);
        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
        }

        if (!ModelState.IsValid || !result.IsValid)
        {
            BackLinkPath = linkGenerator.SocialWorkerRegistrationSelectDisability();
            return Page();
        }

        var socialWorkEnglandRegistrationDate = new DateOnly(
            SocialWorkEnglandRegistrationDate!.Value.Year,
            SocialWorkEnglandRegistrationDate.Value.Month,
            SocialWorkEnglandRegistrationDate.Value.Day);

        var personId = authServiceClient.HttpContextService.GetPersonId();
        await socialWorkerJourneyService.SetSocialWorkEnglandRegistrationDateAsync(personId, socialWorkEnglandRegistrationDate);

        return Redirect(linkGenerator.SocialWorkerRegistrationSexAndGenderIdentity()); // TODO update this to select highest qualification page
    }
}
