using System.Net;
using System.Text.Json;
using DfeSwwEcf.NotificationService.Models;
using DfeSwwEcf.NotificationService.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace DfeSwwEcf.NotificationService;

/// <summary>
/// A HTTP triggered Azure Function for sending notifications
/// </summary>
/// <param name="log"></param>
/// <param name="validator"></param>
/// <param name="notificationCommand"></param>
public class NotificationFunction(
    ILogger<NotificationFunction> log,
    IValidator<NotificationRequest> validator,
    INotificationCommand notificationCommand
)
{
    [Function("Notification")]
    [OpenApiOperation(operationId: "RunAsync", tags: ["Notification Function"])]
    [OpenApiSecurity(
        "function_key",
        SecuritySchemeType.ApiKey,
        Name = "code",
        In = OpenApiSecurityLocationType.Query
    )]
    [OpenApiRequestBody("application/json", typeof(NotificationRequest))]
    [OpenApiResponseWithoutBody(
        statusCode: HttpStatusCode.OK,
        Description = "Notification Sent Successfully"
    )]
    [OpenApiResponseWithoutBody(
        statusCode: HttpStatusCode.BadRequest,
        Description = "Validation Error / Invalid JSON"
    )]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req
    )
    {
        log.LogInformation("C# HTTP trigger function processed a request.");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

        NotificationRequest? data;
        try
        {
            data = JsonSerializer.Deserialize<NotificationRequest>(requestBody);
        }
        catch (JsonException)
        {
            return new BadRequestObjectResult("Request body is not valid json");
        }

        if (data == null)
        {
            return new BadRequestObjectResult("Request body failed to deserialise");
        }

        var validationResults = await validator.ValidateAsync(data);
        if (!validationResults.IsValid)
        {
            return new BadRequestObjectResult(JsonSerializer.Serialize(validationResults.Errors));
        }

        var response = await notificationCommand.SendNotificationAsync(data);

        return new StatusCodeResult((int)response.StatusCode);
    }
}
