using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using System.Threading;
using System.Net;

namespace DfeSwwEcf.NotificationService
{
    public class HealthCheckFunction
    {
        private readonly HealthCheckService _healthCheckService;
        private readonly ILogger<HealthCheckFunction> _logger;

        public HealthCheckFunction(HealthCheckService healthCheckService, ILogger<HealthCheckFunction> logger)
        {
            _healthCheckService = healthCheckService;
            _logger = logger;
        }

        // HTTP Triggered Function to expose the health check endpoint.
        // It's accessible via a GET request to "/api/health".
        [Function("HealthCheck")] // Use [Function] attribute for isolated worker
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health")] HttpRequestData req,
            FunctionContext executionContext)
        {
            _logger.LogInformation("Health check endpoint was hit.");

            var report = await _healthCheckService.CheckHealthAsync();

            var response = req.CreateResponse();

            if (report.Status == HealthStatus.Healthy)
            {
                // If all checks are healthy, return HTTP 200 OK.
                response.StatusCode = HttpStatusCode.OK;
                await response.WriteStringAsync(report.Status.ToString());
            }
            else if (report.Status == HealthStatus.Degraded)
            {
                // If some checks are degraded, return HTTP 200 OK but with a degraded status.
                response.StatusCode = HttpStatusCode.OK;
                await response.WriteStringAsync(report.Status.ToString());
            }
            else // HealthStatus.Unhealthy
            {
                // If any critical checks fail, return HTTP 503 Service Unavailable.
                response.StatusCode = HttpStatusCode.ServiceUnavailable;
                await response.WriteStringAsync(report.Status.ToString());
            }

            return response;
        }
    }
}