using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SocialWorkInductionProgramme.Frontend.Authorisation;
using SocialWorkInductionProgramme.Frontend.HttpClients.AuthService.Models.Pagination;
using SocialWorkInductionProgramme.Frontend.Models;
using SocialWorkInductionProgramme.Frontend.Pages.Shared;
using SocialWorkInductionProgramme.Frontend.Services.Interfaces;

namespace SocialWorkInductionProgramme.Frontend.Pages.ManageAccounts;

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
