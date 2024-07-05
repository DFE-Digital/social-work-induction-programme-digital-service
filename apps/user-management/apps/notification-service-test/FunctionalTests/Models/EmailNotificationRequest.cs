namespace DfeSwwEcf.NotificationService.Tests.FunctionalTests.Models
{
    public class EmailNotificationRequest
    {
        public string EmailAddress { get; set; } = null!;

        public string TemplateId { get; set; } = null!;

        public Dictionary<string, string>? Personalisation { get; set; }
    }
}
