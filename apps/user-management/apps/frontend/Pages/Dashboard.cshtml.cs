using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisations;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages;

[AuthorizeRoles(RoleType.Administrator)]
public class Dashboard : BasePageModel
{
    public IEnumerable<CardItem>? Items { get; set; }

    public PageResult OnGet()
    {
        Items = new List<CardItem>
        {
            new()
            {
                Link = new CardLink
                {
                    Text = "Manage organisations",
                    Path = "/manage-organisations", // TODO replace with path to manage organisations page when built via link generator
                },
                Description = "Add or edit organisations and manage users."
            }
        };
        return Page();
    }
}
