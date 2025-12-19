using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Controllers;

[Route("error")]
public class ErrorController : Controller
{
    public IActionResult Error(int? code)
    {
        var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();
        var statusCodeReExecuteFeature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

        // If there is no error, return a 404 (prevents browsing to this page directly)
        if (exceptionHandlerFeature == null && statusCodeReExecuteFeature == null)
        {
            return NotFound();
        }

        var statusCode = code ?? 500;
        // Recode 403 Forbidden as 404 Not Found, so we don't give away our internal URLs
        if (statusCode == 403)
        {
            statusCode = 404;
        }
        // Recode 500 Internal Server Error as 503 Service Unavailable: a 'better' API response to clients
        if (statusCode == 500)
        {
            statusCode = 503;
        }

        var message = exceptionHandlerFeature?.Error.Message ?? $"HTTP {statusCode} error";

        // Return an object, which will be serialised as JSON
        return new ObjectResult(new
        {
            error = message,
            status = statusCode,
            path = exceptionHandlerFeature?.Path
        })
        {
            StatusCode = statusCode
        };
    }
}
