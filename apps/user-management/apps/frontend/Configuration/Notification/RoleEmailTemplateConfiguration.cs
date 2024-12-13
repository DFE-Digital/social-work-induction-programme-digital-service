namespace Dfe.Sww.Ecf.Frontend.Configuration.Notification;

public class RoleEmailTemplateConfiguration
{
    public required Guid Invitation { get; init; }
    public required Guid Welcome { get; init; }
    public required Guid Pause { get; init; }
    public required Guid Unpause { get; init; }
    public required Guid Link { get; init; }
    public required Guid Unlink { get; init; }
}
