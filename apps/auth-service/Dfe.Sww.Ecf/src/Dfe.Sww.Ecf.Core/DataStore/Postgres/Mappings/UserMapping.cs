using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dfe.Sww.Ecf.Core.DataStore.Postgres.Mappings;

public class UserBaseMapping : IEntityTypeConfiguration<UserBase>
{
    public void Configure(EntityTypeBuilder<UserBase> builder)
    {
        builder.ToTable("users");
        builder.HasKey(e => e.UserId);
        builder.HasDiscriminator(e => e.UserType)
            .HasValue<User>(UserType.Person)
            .HasValue<SystemUser>(UserType.System);
        builder.Property(e => e.UserType).IsRequired();
        builder.Property(e => e.Name).IsRequired().HasMaxLength(UserBase.NameMaxLength);
    }
}

public class UserMapping : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(e => e.Email).HasMaxLength(200).UseCollation("case_insensitive");
        builder.Property(e => e.Roles).HasColumnType("varchar[]");
    }
}

public class SystemUserMapping : IEntityTypeConfiguration<SystemUser>
{
    public void Configure(EntityTypeBuilder<SystemUser> builder)
    {
        builder.HasData(SystemUser.Instance);
    }
}
