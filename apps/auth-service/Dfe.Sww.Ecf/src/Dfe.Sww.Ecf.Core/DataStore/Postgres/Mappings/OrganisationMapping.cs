using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dfe.Sww.Ecf.Core.DataStore.Postgres.Mappings;

public class OrganisationMapping : IEntityTypeConfiguration<Organisation>
{
    public void Configure(EntityTypeBuilder<Organisation> builder)
    {
        builder.ToTable("organisations");
        builder.HasKey(o => o.OrganisationId);
        builder.Property(o => o.OrganisationId).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(o => o.OrganisationName).HasMaxLength(255).IsRequired();
        builder.Property(o => o.CreatedOn).HasDefaultValueSql("now()");
        builder.Property(o => o.Region).HasMaxLength(255);
        builder.Property(p => p.PhoneNumber).HasMaxLength(15).UseCollation("case_insensitive");
    }
}
