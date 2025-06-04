using Dfe.Sww.Ecf.Frontend.Authorisation;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Pagination;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Dfe.Sww.Ecf.Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.ManageUsers;

[AuthorizeRoles(RoleType.Coordinator)]
public class Index(IAccountService accountService) : BasePageModel
{
    [FromQuery]
    public int Offset { get; set; } = 0;

    [FromQuery]
    public int PageSize { get; set; } = 10;

    public IList<Account> Accounts { get; set; } = default!;

    public PaginationMetaData? Pagination { get; set; }

    public async Task<PageResult> OnGetAsync()
    {
        var paginatedResults = await accountService.GetAllAsync(
            new PaginationRequest(Offset, PageSize)
        );

        Accounts = paginatedResults.Records.OrderBy(account => account.CreatedAt).ToList();
        Pagination = paginatedResults.MetaData;

        return Page();
    }
}
