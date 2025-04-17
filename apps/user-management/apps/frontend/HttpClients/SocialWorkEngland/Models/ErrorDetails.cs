using System.Net;

namespace Dfe.Sww.Ecf.Frontend.HttpClients.SocialWorkEngland.Models;

public class ErrorDetails
{
    public string? ErrorMessage { get; set; }

    public HttpStatusCode HttpStatusCode { get; set; }
}
