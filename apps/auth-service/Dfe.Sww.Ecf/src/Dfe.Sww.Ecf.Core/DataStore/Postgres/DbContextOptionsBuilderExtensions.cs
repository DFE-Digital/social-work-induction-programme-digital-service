using Dfe.Sww.Ecf.Core.Infrastructure.Configuration;

namespace Dfe.Sww.Ecf.Core.DataStore.Postgres;

public static class DbContextOptionsBuilderExtensions
{
    public static DbContextOptionsBuilder UseDatabaseSeeding(this DbContextOptionsBuilder builder,
        DatabaseSeedOptions seedOptions
    )
    {
        builder.UseAsyncSeeding(async (db, _, ct) =>
        {
            await DatabaseSeeder.SeedPersonAsync(db, seedOptions.PersonId, seedOptions.OneLoginEmail, ct);
            await DatabaseSeeder.SeedPersonRoleAsync(db, seedOptions.PersonId, seedOptions.RoleId, ct);

            if (seedOptions.OrganisationId != null)
            {
                var orgId = (Guid)seedOptions.OrganisationId;
                await DatabaseSeeder.SeedOrganisationAsync(db, orgId, seedOptions.OrganisationName, ct);
                await DatabaseSeeder.SeedPersonOrganisationAsync(db, orgId, seedOptions.PersonId, ct);
            }

            await db.SaveChangesAsync(ct);
        });
        return builder;
    }
}
