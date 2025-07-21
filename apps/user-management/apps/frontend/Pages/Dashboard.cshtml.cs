using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisations;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages;

[AuthorizeRoles(RoleType.Administrator)]
public class Dashboard(EcfLinkGenerator linkGenerator) : BasePageModel
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
                    Path = linkGenerator.ManageOrganisations.Index()
                },
                Description = "Add or edit organisations and manage users."
            }
        };
        return Page();
    }
}
