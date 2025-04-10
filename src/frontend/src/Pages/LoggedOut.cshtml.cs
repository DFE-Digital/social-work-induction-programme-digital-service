using Microsoft.AspNetCore.Mvc.RazorPages;
using SocialWorkInductionProgramme.Frontend.Pages.Shared;

namespace SocialWorkInductionProgramme.Frontend.Pages;

public class LoggedOut : BasePageModel
{
    public PageResult OnGet()
    {
        return Page();
    }
}
