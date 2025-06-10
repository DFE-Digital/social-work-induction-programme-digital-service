using System.ComponentModel.DataAnnotations;
using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using GovUk.Frontend.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NodaTime;

namespace Dfe.Sww.Ecf.Frontend.Pages.SocialWorkerRegistration;

[AuthorizeRoles(RoleType.EarlyCareerSocialWorker)]
public class DateOfBirth(EcfLinkGenerator linkGenerator) : BasePageModel
{
    [BindProperty]
    [DateInput(ErrorMessagePrefix = "Date of birth")]
    [Required(ErrorMessage = "Enter your date of birth")]
    public LocalDate? UserDateOfBirth { get; set; }

    public PageResult OnGet()
    {
        BackLinkPath = linkGenerator.SocialWorkerRegistration();
        return Page();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            BackLinkPath = linkGenerator.SocialWorkerRegistration();
            return Page();
        }

        return Redirect(linkGenerator.Home()); // TODO update this ECSW sex and gender
    }
}
