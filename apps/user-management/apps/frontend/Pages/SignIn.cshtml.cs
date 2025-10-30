using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Email;
using Dfe.Sww.Ecf.Frontend.Services.Email.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Sww.Ecf.Frontend.Pages;

[Authorize]
public class SignIn(
    EcfLinkGenerator linkGenerator,
    IAuthServiceClient authServiceClient,
    IEmailService emailService
) : BasePageModel
{
    public async Task<IActionResult> OnGetAsync()
    {
        var isEcswRegistered = authServiceClient.HttpContextService.GetIsEcswRegistered();
        if (isEcswRegistered == false)
        {
            return Redirect(linkGenerator.SocialWorkerRegistration());
        }

        if (HttpContext.User.Identity?.IsAuthenticated == true && User.IsInRole(RoleType.Administrator.ToString()))
        {
            return Redirect(linkGenerator.Dashboard());
        }

        var token = Request.Query["linkingToken"].ToString();
        var handler = Request.Query["handler"].ToString();

        if (!string.IsNullOrEmpty(token) && string.Equals(handler, "invite", StringComparison.OrdinalIgnoreCase))
        {
            var personId = authServiceClient.HttpContextService.GetPersonId();
            await emailService.SendWelcomeEmailAsync(new WelcomeEmailRequest
            {
                AccountId = personId
            });
        }
        return Redirect(linkGenerator.Home());
    }
}
