using System.ComponentModel.DataAnnotations.Schema;

namespace Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;

public class Role
{
    public int RoleId { get; set; }

    [Column(TypeName = "varchar(50)")]
    public RoleType RoleName { get; set; }

    // EF Navigation Properties
    public ICollection<PersonRole> PersonRoles { get; set; } = new List<PersonRole>();
}

public enum RoleType
{
    EarlyCareerSocialWorker = 400,
    Assessor = 600,
    Coordinator = 800,
}
