using System.Net;
using DfeSwwEcf.SweApiSimulator.Models;
using DfeSwwEcf.SweApiSimulator.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DfeSwwEcf.SweApiSimulator.Controllers;

/// <summary>
/// Social Worker Controller
/// </summary>
/// <param name="socialWorkerService"></param>
[ApiController]
[Route("[controller]")]
public class SocialWorkerController(ISocialWorkerService socialWorkerService) : ControllerBase
{
    private readonly ISocialWorkerService _socialWorkerService = socialWorkerService;

    /// <summary>
    /// Get a social worker by their ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns>A single social worker record</returns>
    [HttpGet("{id?}")]
    public IActionResult GetById(string? id)
    {
        var response = _socialWorkerService.GetById(id);

        if (response?.ErrorDetails is not null)
        {
            object? result =
                response.ErrorDetails.HttpStatusCode == HttpStatusCode.UnprocessableEntity
                    ? new NonIntSweIdResponse { Error = response.ErrorDetails.ErrorMessage }
                    : response.ErrorDetails.ErrorMessage;

            return new ObjectResult(result)
            {
                StatusCode = (int)response.ErrorDetails.HttpStatusCode
            };
        }

        return new OkObjectResult(response?.SocialWorker);
    }
}
