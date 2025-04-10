using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialWorkInductionProgramme.Frontend.Pages.Shared;

namespace SocialWorkInductionProgramme.Frontend.Pages;

[Authorize]
public class SignIn : BasePageModel
{
    public RedirectToPageResult OnGet()
    {
        return RedirectToPage(nameof(Index));
    }
}
