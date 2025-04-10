using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SocialWorkInductionProgramme.Frontend.Pages.Shared;

public class BasePageModel : PageModel
{
    public string? Title { get; set; }

    public string? BackLinkPath { get; set; }
}
