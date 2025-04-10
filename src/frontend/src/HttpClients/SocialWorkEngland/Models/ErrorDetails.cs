using System.Net;

namespace SocialWorkInductionProgramme.Frontend.HttpClients.SocialWorkEngland.Models;

public class ErrorDetails
{
    public string? ErrorMessage { get; set; }

    public HttpStatusCode HttpStatusCode { get; set; }
}
