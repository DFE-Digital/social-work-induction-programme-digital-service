using System.Net;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Models
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
