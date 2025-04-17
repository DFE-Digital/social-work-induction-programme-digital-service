using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dfe.Sww.Ecf.Core.DataStore.Postgres.Mappings;

public class RoleMapping : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        var roleNames = string.Join(", ", Enum.GetNames<RoleType>().Select(v => $"'{v}'"));
        builder.ToTable(
            "roles",
            t =>
            {
                t.HasCheckConstraint("ck_roles_role_name", $"role_name in ({roleNames})");
            }
        );

        builder.HasKey(r => r.RoleId).HasName("pk_roles");

        builder.Property(r => r.RoleName).HasMaxLength(50).IsRequired();

        builder.HasIndex(r => r.RoleName).IsUnique().HasDatabaseName("uq_roles_role_name");
    }
}
