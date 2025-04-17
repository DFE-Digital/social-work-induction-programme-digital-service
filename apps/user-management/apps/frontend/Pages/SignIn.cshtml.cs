using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Sww.Ecf.Frontend.Pages;

[Authorize]
public class SignIn : BasePageModel
{
    public RedirectToPageResult OnGet()
    {
        return RedirectToPage(nameof(Index));
    }
}
