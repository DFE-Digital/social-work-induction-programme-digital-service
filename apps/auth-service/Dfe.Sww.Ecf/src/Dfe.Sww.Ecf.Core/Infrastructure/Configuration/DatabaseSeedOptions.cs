namespace Dfe.Sww.Ecf.Core.Infrastructure.Configuration;

public class DatabaseSeedOptions
{
    public Guid OrganisationId { get; set; }
    public string OrganisationName { get; set; } = string.Empty;
    public Guid PersonId { get; set; }
    public int RoleId { get; set; }
}
