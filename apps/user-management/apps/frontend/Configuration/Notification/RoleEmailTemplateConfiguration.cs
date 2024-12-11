namespace Dfe.Sww.Ecf.Frontend.Configuration.Notification;

public class RoleEmailTemplateConfiguration
{
    public required Guid Invitation { get; init; }
    public required Guid Welcome { get; init; }
    public Guid Pause { get; init; }
    public Guid Unpause { get; init; }
}
