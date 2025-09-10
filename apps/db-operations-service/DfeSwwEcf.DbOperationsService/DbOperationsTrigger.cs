using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using JetBrains.Annotations;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization; // Added for [Required] attribute

namespace DfeSwwEcf.DbOperationsService
{
    public class DbOperationsFunction(ILoggerFactory loggerFactory)
    {
        private readonly ILogger _logger = loggerFactory.CreateLogger<DbOperationsFunction>();

        [Function("DbOperationsFunction")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
            CancellationToken cancellationToken)
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

                // Handle async requests
                if (data.Async)
                {
                    var jobId = StartAsyncJob(data);
                    var statusUrl = $"{req.Url.Scheme}://{req.Url.Host}/api/DbOperationsStatus?jobId={jobId}";
                    _logger.LogInformation("Started async job {JobId} for action '{Action}'", jobId, data.Action);

                    var asyncResponse = new StatusResponse(
                        Status: Status.Accepted,
                        JobId: jobId,
                        Message: "Job started asynchronously",
                        StatusUrl: statusUrl,
                        ExitCode: null,
                        Output: null,
                        Error: null
                    );

                    return await CreateJsonResponse(req, HttpStatusCode.Accepted, asyncResponse);
                }

                // Handle synchronous requests (existing logic)
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

                // Execute the shell script synchronously
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

        private static string StartAsyncJob(JobRequest data)
        {
            // Clean up old jobs periodically
            AsyncJobTracker.CleanupOldJobs();

            // Prepare the script execution
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
                    throw new ArgumentException("'BackupFileName' is required when 'action' is 'restore'.");
                }

                scriptPath = "/app/scripts/run-restore.sh";
                arguments = [data.DatabaseName, data.StorageAccount, data.ContainerName, data.BackupFileName];
            }
            else
            {
                throw new ArgumentException("Invalid action specified. Must be 'backup' or 'restore'.");
            }

            // Start the job asynchronously (without a cancellation token to prevent timeout)
            var jobTask = ExecuteShellScript(scriptPath, arguments, CancellationToken.None);
            var jobId = AsyncJobTracker.StartJob(jobTask, data);

            return jobId;
        }

        [Function("DbOperationsStatus")]
        public static async Task<HttpResponseData> GetStatus(
            [HttpTrigger(AuthorizationLevel.Function, "get")]
            HttpRequestData req)
        {
            var jobId = req.Query["jobId"];

            if (string.IsNullOrEmpty(jobId))
            {
                var errorResponse = new StatusResponse(
                    Status: Status.Error,
                    JobId: null,
                    Message: "jobId query parameter is required.",
                    StatusUrl: null,
                    ExitCode: null,
                    Output: null,
                    Error: null
                );
                return await CreateJsonResponse(req, HttpStatusCode.BadRequest, errorResponse);
            }

            var (exists, completed, exitCode, output, error) = AsyncJobTracker.GetJobStatus(jobId);

            if (!exists)
            {
                var notFoundResponse = new StatusResponse(
                    Status: Status.NotFound,
                    JobId: jobId,
                    Message: $"Job {jobId} not found.",
                    StatusUrl: null,
                    ExitCode: null,
                    Output: null,
                    Error: null
                );
                return await CreateJsonResponse(req, HttpStatusCode.NotFound, notFoundResponse);
            }

            if (!completed)
            {
                var runningResponse = new StatusResponse(
                    Status: Status.Running,
                    JobId: jobId,
                    Message: "Job is still running.",
                    StatusUrl: null,
                    ExitCode: null,
                    Output: null,
                    Error: null
                );
                return await CreateJsonResponse(req, HttpStatusCode.OK, runningResponse);
            }

            // Job is completed
            var completedResponse = new StatusResponse(
                Status: exitCode == 0 ? Status.Completed : Status.Failed,
                JobId: jobId,
                Message: exitCode == 0 ? "Job completed successfully." : $"Job failed with exit code {exitCode}.",
                StatusUrl: null,
                ExitCode: exitCode,
                Output: output,
                Error: error
            );

            return await CreateJsonResponse(req, exitCode == 0 ? HttpStatusCode.OK : HttpStatusCode.InternalServerError,
                completedResponse);
        }

        private static async Task<HttpResponseData> CreateResponse(HttpRequestData req, HttpStatusCode status,
            string body)
        {
            var response = req.CreateResponse(status);
            await response.WriteStringAsync(body);
            return response;
        }

        private static async Task<HttpResponseData> CreateJsonResponse(HttpRequestData req, HttpStatusCode status,
            StatusResponse statusResponse)
        {
            var response = req.CreateResponse(status);
            response.Headers.Add("Content-Type", "application/json");
            await response.WriteStringAsync(JsonSerializer.Serialize(statusResponse, JsonOptions));
            return response;
        }

        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower) }
        };
    }

    public record JobRequest
    {
        [Required] public required string Action { get; init; }

        [Required] public required string DatabaseName { get; init; }

        [Required] public required string StorageAccount { get; init; }

        [Required] public required string ContainerName { get; init; }

        public string? BackupFileName { get; init; }

        public bool Async { get; init; }
    }

    [UsedImplicitly]
    public record StatusResponse(
        Status Status, // "running", "completed", "failed", "not_found", "error", "accepted"
        string? JobId,
        string Message,
        string? StatusUrl, // For async job start responses
        int? ExitCode,
        string? Output,
        string? Error
    );

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Status
    {
        Running,
        Completed,
        Failed,
        NotFound,
        Error,
        Accepted
    }

    public static class AsyncJobTracker
    {
        private static readonly ConcurrentDictionary<string, JobInfo> Jobs = new();

        private static readonly Timer CleanupTimer = new(CleanupOldJobs, null,
            TimeSpan.FromHours(1), TimeSpan.FromHours(1));
        
        private record JobInfo(Task<(int, string, string)> Task, DateTime StartTime, JobRequest Request);

        public record JobStatus(bool Exists, bool Completed, int? ExitCode, string? Output, string? Error);


        public static string StartJob(Task<(int, string, string)> jobTask, JobRequest request)
        {
            var jobId = Guid.NewGuid().ToString();
            Jobs[jobId] = new JobInfo(jobTask, DateTime.UtcNow, request);
            return jobId;
        }


        public static JobStatus GetJobStatus(
            string jobId)
        {
            if (!Jobs.TryGetValue(jobId, out var jobInfo))
                return new JobStatus(false, false, null, null, null);

            if (!jobInfo.Task.IsCompleted)
                return new JobStatus(true, false, null, null, null);

            var (exitCode, output, error) = jobInfo.Task.Result;
            return new JobStatus(true, true, exitCode, output, error);
        }

        public static void CleanupOldJobs(object? state = null)
        {
            var cutoffTime = DateTime.UtcNow.AddHours(-24); // Keep jobs for 24 hours
            var oldJobIds = Jobs.Where(kvp => kvp.Value.StartTime < cutoffTime).Select(kvp => kvp.Key).ToList();

            foreach (var jobId in oldJobIds)
            {
                if (!Jobs.TryRemove(jobId, out var jobInfo)) continue;
                // Optionally dispose of any resources if the task is still running
                if (!jobInfo.Task.IsCompleted)
                {
                    // Log that we're cleaning up a still-running job
                    // Consider if you want to cancel it or just remove tracking
                }
            }

            if (oldJobIds.Count > 0)
            {
                // Log cleanup activity (you'll need to pass a logger or use a static logger)
                Console.WriteLine($"Cleaned up {oldJobIds.Count} old jobs");
            }
        }
    }
}