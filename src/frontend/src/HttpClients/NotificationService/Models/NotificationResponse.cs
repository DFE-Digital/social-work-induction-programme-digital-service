using System.Net;

namespace SocialWorkInductionProgramme.Frontend.HttpClients.NotificationService.Models
{
    /// <summary>
    /// The Notification Response
    /// </summary>
    public class NotificationResponse
    {
        /// <summary>
        /// The HTTP response code
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
    }
}
