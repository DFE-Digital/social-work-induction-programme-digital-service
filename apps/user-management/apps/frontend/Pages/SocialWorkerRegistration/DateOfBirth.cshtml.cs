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
public class DateOfBirth(
    EcfLinkGenerator linkGenerator,
    IRegisterSocialWorkerJourneyService socialWorkerJourneyService,
    IAuthServiceClient authServiceClient,
    IValidator<DateOfBirth> validator)
    : BasePageModel
{
    [BindProperty]
    [DateInput(DateInputItemTypes.All, ErrorMessagePrefix = "Date of birth")]
    [Required(ErrorMessage = "Enter your date of birth")]
    public LocalDate? UserDateOfBirth { get; set; }

    public async Task<PageResult> OnGetAsync()
    {
        var person = authServiceClient.HttpContextService.GetPersonId();
        var dob = await socialWorkerJourneyService.GetDateOfBirthAsync(person);
        if (dob.HasValue)
        {
            UserDateOfBirth = LocalDate.FromDateTime(dob.Value);
        }

        BackLinkPath = linkGenerator.SocialWorkerRegistration();
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
            BackLinkPath = linkGenerator.SocialWorkerRegistration();
            return Page();
        }

        var dateOfBirth = new DateTime(
            UserDateOfBirth!.Value.Year,
            UserDateOfBirth.Value.Month,
            UserDateOfBirth.Value.Day);

        var person = authServiceClient.HttpContextService.GetPersonId();
        await socialWorkerJourneyService.SetDateOfBirthAsync(person, dateOfBirth);

        return Redirect(linkGenerator.Home()); // TODO update this ECSW sex and gender page
    }
}
