using Dfe.Sww.Ecf.Frontend.Models;

namespace Dfe.Sww.Ecf.Frontend.Configuration.Notification;

public class EmailTemplateOptions
{
    public required Dictionary<AccountType, RoleEmailTemplateConfiguration> Roles { get; init; }
    public required Guid PrimaryCoordinatorInvitationEmail { get; init; }
}
