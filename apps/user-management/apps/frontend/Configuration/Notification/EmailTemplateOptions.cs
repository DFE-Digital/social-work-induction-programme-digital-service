namespace Dfe.Sww.Ecf.Frontend.Configuration.Notification;

public class EmailTemplateOptions
{
    public required Guid Invitation { get; init; }
    public required Guid PrimaryCoordinatorInvitationEmail { get; init; }
}
