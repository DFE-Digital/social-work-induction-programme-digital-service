namespace DfeSwwEcf.NotificationService.Tests.FunctionalTests.Models
{
    public class EmailNotificationRequest
    {
        public string EmailAddress { get; set; } = null!;

        public Guid TemplateId { get; set; }

        public Dictionary<string, string>? Personalisation { get; set; }
    }
}
