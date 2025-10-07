using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dfe.Sww.Ecf.Core.DataStore.Postgres.Mappings;

public class PersonMapping : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("persons");
        builder.HasKey(p => p.PersonId);
        builder.Property(p => p.PersonId).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(o => o.CreatedOn).HasDefaultValueSql("now()");
        builder.HasIndex(p => p.Trn).HasFilter("trn is not null").IsUnique();
        builder.Property(p => p.Trn).IsFixedLength();
        builder.Property(p => p.FirstName).UseCollation("case_insensitive");
        builder.Property(p => p.MiddleName).UseCollation("case_insensitive");
        builder.Property(p => p.LastName).UseCollation("case_insensitive");
        builder.Property(p => p.EmailAddress).UseCollation("case_insensitive");
        builder.Property(p => p.NationalInsuranceNumber).IsFixedLength();
        builder.Property(p => p.OtherGenderIdentity).UseCollation("case_insensitive");
        builder.Property(p => p.OtherEthnicGroupWhite).UseCollation("case_insensitive");
        builder.Property(p => p.OtherEthnicGroupAsian).UseCollation("case_insensitive");
        builder.Property(p => p.OtherEthnicGroupMixed).UseCollation("case_insensitive");
        builder.Property(p => p.OtherEthnicGroupBlack).UseCollation("case_insensitive");
        builder.Property(p => p.OtherEthnicGroupOther).UseCollation("case_insensitive");
        builder.Property(p => p.OtherRouteIntoSocialWork).UseCollation("case_insensitive");
    }
}
