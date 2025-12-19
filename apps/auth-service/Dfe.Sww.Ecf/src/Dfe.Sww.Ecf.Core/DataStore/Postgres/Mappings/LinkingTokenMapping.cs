using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dfe.Sww.Ecf.Core.DataStore.Postgres.Mappings;

public class LinkingTokenMapping : IEntityTypeConfiguration<LinkingToken>
{
    public void Configure(EntityTypeBuilder<LinkingToken> builder)
    {
        builder.ToTable("linking_token");
        builder.HasKey(lt => lt.LinkingTokenId);
        builder.Property(lt => lt.LinkingTokenId).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(lt => lt.Token).IsRequired().IsFixedLength();
        builder.Property(lt => lt.CreatedOn).IsRequired().HasDefaultValueSql("now()");
        builder
            .Property(lt => lt.ExpirationOn)
            .IsRequired()
            .HasDefaultValueSql("now() + interval '3 days'");

        builder
            .HasOne(lt => lt.Person)
            .WithMany(p => p.LinkingTokens)
            .OnDelete(DeleteBehavior.Cascade)
            .HasForeignKey(lt => lt.PersonId);
    }
}
