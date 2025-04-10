namespace SocialWorkInductionProgramme.Frontend.Configuration.Notification;

public class EmailTemplateOptions
{
    public required Dictionary<string, RoleEmailTemplateConfiguration> Roles { get; init; }
}
