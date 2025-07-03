namespace Dfe.Sww.Ecf.Core.Infrastructure.Configuration;

public class DatabaseSeedOptions
{
    public Guid OrganisationId { get; init; }
    public string OrganisationName { get; init; } = string.Empty;
    public Guid PersonId { get; init; }
    public int RoleId { get; init; }
    public string OneLoginSubject { get; init; } = string.Empty;
    public string OneLoginEmail { get; init; } = string.Empty;
}
