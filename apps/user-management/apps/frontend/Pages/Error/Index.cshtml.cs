using Dfe.Sww.Ecf.Frontend.Pages.Shared;

namespace Dfe.Sww.Ecf.Frontend.Pages.Error;

public class ErrorModel : BasePageModel
{
    public string? PartialName { get; set; }
    public int? ErrorStatusCode { get; set; }

    public void OnGet(int? code = null)
    {
        ErrorStatusCode = code;

        switch (ErrorStatusCode)
        {
            case 403:
                PartialName = "NotAuthorised";
                Title = "You do not have permission to view this page";
                break;
            case 404:
                PartialName = "PageNotFound";
                Title = "Page not found";
                break;
            case 500:
                PartialName = "ProblemWithTheService";
                Title = "Sorry, there is a problem with the service";
                break;
            default:
                Title = "An unexpected error occurred.";
                break;
        }
    }
}
