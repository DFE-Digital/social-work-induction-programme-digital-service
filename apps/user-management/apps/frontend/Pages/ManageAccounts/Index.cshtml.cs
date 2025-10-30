using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Interfaces;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Pagination;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Routing;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageAccounts;

public class Index(IAccountService accountService, IOrganisationService organisationService, EcfLinkGenerator linkGenerator) : ManageAccountsBasePageModel
{
    [FromQuery]
    public int Offset { get; set; } = 0;

    [FromQuery]
    public int PageSize { get; set; } = 10;

    public IList<Account> Accounts { get; set; } = default!;

    public PaginationMetaData? Pagination { get; set; }

    public Organisation? Organisation { get; set; }

    public async Task<PageResult> OnGetAsync()
    {
        if (User.IsInRole(nameof(RoleType.Administrator)))
        {
            BackLinkPath = linkGenerator.ManageOrganisations.Index();
        }

        Organisation = await organisationService.GetByIdAsync(OrganisationId);

        var paginatedResults = await accountService.GetAllAsync(
            new PaginationRequest(Offset, PageSize),
            OrganisationId
        );

        Accounts = paginatedResults.Records.ToList();
        Pagination = paginatedResults.MetaData;

        return Page();
    }
}
