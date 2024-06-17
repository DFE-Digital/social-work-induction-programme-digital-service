using System.Diagnostics;
using System.Net;
using DfeSwwEcf.NotificationService.Models;
using DfeSwwEcf.NotificationService.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Notify.Exceptions;
using Notify.Interfaces;
using Polly;
using Polly.Contrib.WaitAndRetry;

namespace DfeSwwEcf.NotificationService.Services;

/// <inheritdoc/>
public class EmailNotificationCommand(
    IAsyncNotificationClient notificationClient,
    ILogger<EmailNotificationCommand> log) : INotificationCommand
{
    /// <inheritdoc/>
    public async Task<NotificationResponse> SendNotificationAsync(NotificationRequest notificationRequest)
    {
        var delay = Backoff.ExponentialBackoff(TimeSpan.FromSeconds(1), retryCount: 5);
        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(delay);

        var result = await retryPolicy.ExecuteAndCaptureAsync(GovNotifyCall);
        if (result.FinalException != null)
        {
            var httpResponse = new NotificationResponse
            {
                StatusCode = result.FinalException.Message switch
                {
                    { } a when a.Contains(GovNotifyExceptionConstants.EXCEPTION) => HttpStatusCode.InternalServerError,
                    { } a when a.Contains(GovNotifyExceptionConstants.RATE_LIMIT_ERROR) => HttpStatusCode.TooManyRequests,
                    _ => HttpStatusCode.InternalServerError
                }
            };
            log.LogError("{errorMessage} GovNotify error mapped to {statusCode}", result.FinalException.Message, httpResponse.StatusCode);
            return httpResponse;
        }

        return result.Result;

        /// <summary>
        /// Makes a GOV Notify Call, catches and processes any exceptions
        /// </summary>
        /// <returns>NotificationResponse with status code</returns>
        async Task<NotificationResponse> GovNotifyCall()
        {
            try
            {
                await notificationClient.SendEmailAsync(
                    emailAddress: notificationRequest.EmailAddress,
                    templateId: notificationRequest.TemplateId.ToString(),
                    personalisation: notificationRequest.Personalisation?.ToDictionary(kvp => kvp.Key, kvp => (dynamic)kvp.Value) ?? null,
                    clientReference: notificationRequest?.Reference ?? null,
                    emailReplyToId: notificationRequest?.EmailReplyToId?.ToString() ?? null);

                return new NotificationResponse { StatusCode = HttpStatusCode.OK };
            }
            catch (NotifyClientException ex)
            {
                if (ex.Message.Contains(GovNotifyExceptionConstants.EXCEPTION, StringComparison.InvariantCultureIgnoreCase) ||
                    ex.Message.Contains(GovNotifyExceptionConstants.RATE_LIMIT_ERROR, StringComparison.InvariantCultureIgnoreCase))
                {
                    // retry for 500 errors or 429 (exceeded rate limit of GOV Notify)
                    throw;
                }

                // no retry for other exception types; map and return
                var httpResponse = new NotificationResponse
                {
                    StatusCode = ex.Message switch
                    {
                        { } a when a.Contains(GovNotifyExceptionConstants.BAD_REQUEST_ERROR) => HttpStatusCode.BadRequest,
                        { } a when a.Contains(GovNotifyExceptionConstants.AUTH_ERROR) => HttpStatusCode.InternalServerError,
                        { } a when a.Contains(GovNotifyExceptionConstants.TOO_MANY_REQUESTS_ERROR) => HttpStatusCode.TooManyRequests,
                        _ => HttpStatusCode.InternalServerError
                    }
                };

                log.LogError("{errorMessage} GovNotify error mapped to {statusCode}", ex.Message, httpResponse.StatusCode);
                return httpResponse;
            }
        }
    }
}
