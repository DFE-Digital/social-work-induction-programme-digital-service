using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages.Shared;

public class BasePageModel : PageModel
{
    public string? Title { get; set; }

    public string? BackLinkPath { get; set; }

    public bool FromChangeLink { get; set; }
}
