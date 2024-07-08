using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Sww.Ecf.UiCommon.FormFlow;

public delegate IActionResult MissingInstanceHandler(
    JourneyDescriptor journeyDescriptor,
    HttpContext httpContext,
    int? statusCode);
