using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;

namespace Dfe.Sww.Ecf.Core.DataStore.Postgres.Mappings;

public class OrganisationMapping : IEntityTypeConfiguration<Organisation>
{
    public void Configure(EntityTypeBuilder<Organisation> builder)
    {
        builder.ToTable("organisations");
        builder.HasKey(o => o.OrganisationId);
        builder.Property(o => o.OrganisationId).HasDefaultValueSql("gen_random_uuid()");
        builder.HasIndex(o => o.ExternalOrganisationId).HasDatabaseName(Organisation.ExternalOrganisationIdIndexName);
        builder.Property(o => o.ExternalOrganisationId).IsRequired();
        builder.Property(o => o.OrganisationName).HasMaxLength(255).IsRequired();
        builder.Property(o => o.CreatedOn).HasDefaultValueSql("now()");
    }
}
