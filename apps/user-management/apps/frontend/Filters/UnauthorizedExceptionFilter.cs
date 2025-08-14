using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dfe.Sww.Ecf.Frontend.Filters;

public class UnauthorizedExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is HttpRequestException httpException && httpException.StatusCode == HttpStatusCode.Unauthorized)
        {
            context.ExceptionHandled = true;
            context.Result = new ChallengeResult();
        }
    }
}
