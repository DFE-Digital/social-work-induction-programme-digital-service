using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Pagination;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.SocialWorkerRegistration;

[AuthorizeRoles(RoleType.EarlyCareerSocialWorker)]
public class Index : BasePageModel
{
    public PageResult OnGet()
    {
        return Page();
    }
}
