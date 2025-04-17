using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;

namespace Dfe.Sww.Ecf.Core.DataStore.Postgres.Mappings;

public class PersonOrganisationMapping : IEntityTypeConfiguration<PersonOrganisation>
{
    public void Configure(EntityTypeBuilder<PersonOrganisation> builder)
    {
        builder.ToTable("person_organisations");
        builder.HasKey(e => e.PersonOrganisationId);
        builder.Property(pr => pr.PersonOrganisationId).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(e => e.StartDate).IsRequired().HasDefaultValueSql("now()");
        builder.Property(e => e.CreatedOn).IsRequired().HasDefaultValueSql("now()");
        builder.HasIndex(e => e.PersonId).HasDatabaseName(PersonOrganisation.PersonIdIndexName);
        builder.HasIndex(e => e.OrganisationId).HasDatabaseName(PersonOrganisation.OrganisationIdIndexName);
        builder
            .HasOne(po => po.Person)
            .WithMany(p => p.PersonOrganisations)
            .HasForeignKey(po => po.PersonId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_person_organisations_person_id");

        builder
            .HasOne(po => po.Organisation)
            .WithMany(r => r.PersonOrganisations)
            .HasForeignKey(pr => pr.OrganisationId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_person_organisations_organisation_id");
    }
}
