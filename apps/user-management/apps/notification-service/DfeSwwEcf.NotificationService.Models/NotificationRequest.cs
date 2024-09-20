namespace DfeSwwEcf.NotificationService.Models;

/// <summary>
/// The Notification Request
/// </summary>
public class NotificationRequest
{
    /// <summary>
    /// The email address the notification is being sent to
    /// </summary>
    public string EmailAddress { get; set; } = null!;

    /// <summary>
    /// The template ID of the notification being sent
    /// </summary>
    public Guid TemplateId { get; set; }

    /// <summary>
    /// The reference to a single email or batch of emails
    /// </summary>
    public string? Reference { get; set; }

    /// <summary>
    /// An email address specified by you to receive replies from your users
    /// </summary>
    public Guid? EmailReplyToId { get; set; }

    /// <summary>
    /// Used to replace placeholders in the template for relevant information
    /// </summary>
    public Dictionary<string, string>? Personalisation { get; set; }
}
