using Dfe.Sww.Ecf.Frontend.Pages.Shared;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages;

public class LoggedOut : BasePageModel
{
    public PageResult OnGet()
    {
        return Page();
    }
}
