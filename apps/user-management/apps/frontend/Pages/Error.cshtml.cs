using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Sww.Ecf.Frontend.Pages;

public class ErrorModel : PageModel
{
    public int? ErrorStatusCode { get; set; }
    public string Message { get; set; } = "An unknown error occurred.";

    public void OnGet(int? code = null)
    {
        ErrorStatusCode = code ?? 500;

        Message = ErrorStatusCode switch
        {
            404 => "The page you are looking for could not be found.",
            403 => "You do not have permission to view this page.",
            500 => "An internal server error occurred.",
            _ => "An unexpected error occurred."
        };
    }
}
