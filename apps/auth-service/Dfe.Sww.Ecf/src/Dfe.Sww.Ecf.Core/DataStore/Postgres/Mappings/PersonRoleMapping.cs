using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dfe.Sww.Ecf.Core.DataStore.Postgres.Mappings;

public class PersonRoleMapping : IEntityTypeConfiguration<PersonRole>
{
    public void Configure(EntityTypeBuilder<PersonRole> builder)
    {
        builder.ToTable("person_roles");
        builder.HasKey(pr => pr.PersonRoleId).HasName("pk_person_roles");
        builder.Property(pr => pr.PersonRoleId).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(pr => pr.AssignedOn).HasDefaultValueSql("now()");

        builder
            .HasOne(pr => pr.Person)
            .WithMany(p => p.PersonRoles)
            .HasForeignKey(pr => pr.PersonId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_person_roles_persons_person_id");

        builder
            .HasOne(pr => pr.Role)
            .WithMany(r => r.PersonRoles)
            .HasForeignKey(pr => pr.RoleId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_person_roles_roles_role_id");

        builder
            .HasIndex(pr => new { pr.PersonId, pr.RoleId })
            .IsUnique()
            .HasDatabaseName("uq_person_roles_person_id_role_id");
    }
}
