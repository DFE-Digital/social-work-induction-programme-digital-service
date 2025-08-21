namespace Dfe.Sww.Ecf.Frontend.HttpClients.NotificationService.Models;

public record NotificationRequest
{
    public required string EmailAddress { get; set; }

    public required Guid TemplateId { get; set; }

    public string? Reference { get; set; }

    public Guid? EmailReplyToId { get; set; }

    public Dictionary<string, string>? Personalisation { get; set; }
}
