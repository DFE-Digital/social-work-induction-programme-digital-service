using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Pagination;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageOrganisations;

[AuthorizeRoles(RoleType.Administrator)]
public class Index(IOrganisationService organisationService) : BasePageModel
{
    [FromQuery]
    public int Offset { get; set; } = 0;

    [FromQuery]
    public int PageSize { get; set; } = 10;

    public IList<Organisation>? Organisations { get; set; }

    public PaginationMetaData? Pagination { get; set; }

    public async Task<PageResult> OnGetAsync()
    {
        var paginatedResults = await organisationService.GetAllAsync(
            new PaginationRequest(Offset, PageSize)
        );

        Organisations = paginatedResults.Records.ToList();
        Pagination = paginatedResults.MetaData;

        return Page();
    }
}
