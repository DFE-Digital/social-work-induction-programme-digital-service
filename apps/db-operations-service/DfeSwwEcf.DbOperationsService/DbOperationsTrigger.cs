using System.Diagnostics;
using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace DfeSwwEcf.DbOperationsService
{
    public class DbOperationsFunction(ILoggerFactory loggerFactory)
    {
        private readonly ILogger _logger = loggerFactory.CreateLogger<DbOperationsFunction>();

        [Function("DbOperationsFunction")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req, CancellationToken cancellationToken)
        {
            _logger.LogInformation("HTTP trigger function received a job request.");

            try
            {
                string requestBody;
                using (var reader = new StreamReader(req.Body))
                {
                    requestBody = await reader.ReadToEndAsync(cancellationToken);
                }

                _logger.LogInformation("Received raw request body: {RequestBody}", requestBody);

                var data = JsonSerializer.Deserialize<JobRequest>(requestBody, JsonOptions);

                if (data == null || string.IsNullOrEmpty(data.Action))
                {
                    return await CreateResponse(req, HttpStatusCode.BadRequest,
                        "Invalid request body. 'action' is required.");
                }

                List<string> arguments;
                string scriptPath;

                if (data.Action.Equals("backup", StringComparison.OrdinalIgnoreCase))
                {
                    scriptPath = "/app/scripts/run-backup.sh";
                    arguments = [data.DatabaseName, data.StorageAccount, data.ContainerName];
                }
                else if (data.Action.Equals("restore", StringComparison.OrdinalIgnoreCase))
                {
                    if (data.BackupFileName == null)
                    {
                        return await CreateResponse(req, HttpStatusCode.BadRequest,
                            "'BackupFileName' is required when 'action' is 'restore'.");
                    }

                    scriptPath = "/app/scripts/run-restore.sh";
                    arguments = [data.DatabaseName, data.StorageAccount, data.ContainerName, data.BackupFileName];
                }
                else
                {
                    return await CreateResponse(req, HttpStatusCode.BadRequest,
                        "Invalid action specified. Must be 'backup' or 'restore'.");
                }

                // Execute the shell script
                var (exitCode, output, error) = await ExecuteShellScript(scriptPath, arguments, cancellationToken);

                if (exitCode != 0)
                {
                    _logger.LogError("Script failed with exit code {ExitCode}.", exitCode);
                    _logger.LogError("STDOUT: {Output}", output);
                    _logger.LogError("STDERR: {Error}", error);
                    return await CreateResponse(req, HttpStatusCode.InternalServerError,
                        $"Script execution failed. See logs for details.");
                }

                _logger.LogInformation("Action '{DataAction}' completed successfully.\nSTDOUT: {Output}", data.Action,
                    output);
                return await CreateResponse(req, HttpStatusCode.OK,
                    $"Action '{data.Action}' completed successfully.\n\nOutput:\n{output}");
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "Failed to deserialize request body.");
                return await CreateResponse(req, HttpStatusCode.BadRequest, "Invalid JSON format in request body.");
            }
            catch (IOException ioEx)
            {
                _logger.LogError(ioEx, "Failed to read request body.");
                return await CreateResponse(req, HttpStatusCode.BadRequest, "Failed to read request body.");
            }
            catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning(ex, "Operation was canceled.");
                throw;
            }
            catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogError(ex, "An unexpected error occurred.");
                return await CreateResponse(req, HttpStatusCode.InternalServerError,
                    $"An unexpected error occurred: {ex.Message}");
            }
        }

        private static async Task<(int ExitCode, string Output, string Error)> ExecuteShellScript(string scriptPath,
            List<string> args, CancellationToken cancellationToken)
        {
            using var process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName = scriptPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            foreach (var arg in args)
            {
                process.StartInfo.ArgumentList.Add(arg);
            }

            process.Start();

            await using var registration = cancellationToken.Register(static state =>
            {
                var p = (Process)state!;
                try
                {
                    if (!p.HasExited)
                    {
                        p.Kill(entireProcessTree: true);
                    }
                }
                catch
                {
                    // Best-effort: the process may have already exited or become non-accessible.
                    // // Do not throw from a cancellation callback.
                }
            }, process);


            var output = await process.StandardOutput.ReadToEndAsync(cancellationToken);
            var error = await process.StandardError.ReadToEndAsync(cancellationToken);
            await process.WaitForExitAsync(cancellationToken);

            return (process.ExitCode, output, error);
        }

        private static async Task<HttpResponseData> CreateResponse(HttpRequestData req, HttpStatusCode status,
            string body)
        {
            var response = req.CreateResponse(status);
            await response.WriteStringAsync(body);
            return response;
        }

        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
        {
            // Explicit for clarity; Web defaults already enable case-insensitive matching.
            PropertyNameCaseInsensitive = true
        };
    }

    public class JobRequest
    {
        public required string Action { get; init; }
        public required string DatabaseName { get; init; }
        public required string StorageAccount { get; init; }
        public required string ContainerName { get; init; }
        public string? BackupFileName { get; init; }
    }
}