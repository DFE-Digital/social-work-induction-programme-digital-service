namespace Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;

public class PersonRole
{
    public Guid PersonRoleId { get; set; }
    public Guid PersonId { get; set; }
    public int RoleId { get; set; }
    public DateTime AssignedOn { get; set; }

    // EF Navigation Properties
    public Person Person { get; set; } = null!;
    public Role Role { get; set; } = null!;
}
