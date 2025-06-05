using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageUsers;

[AuthorizeRoles(RoleType.Coordinator)]
public class ViewUserDetails(IUserService userService, EcfLinkGenerator linkGenerator)
    : BasePageModel
{
    public User UserAccount { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var user = await userService.GetByIdAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        BackLinkPath = linkGenerator.ManageUsers();
        UserAccount = user;

        return Page();
    }
}
