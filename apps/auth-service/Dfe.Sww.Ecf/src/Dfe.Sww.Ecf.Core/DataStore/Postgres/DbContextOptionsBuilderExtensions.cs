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
            await DatabaseSeeder.SeedOrganisationAsync(db, seedOptions.OrganisationId, ct);
            await DatabaseSeeder.SeedPersonAsync(db, seedOptions.PersonId, ct);
            await DatabaseSeeder.SeedPersonOrganisationAsync(db, seedOptions.OrganisationId, seedOptions.PersonId, ct);
            await DatabaseSeeder.SeedPersonRoleAsync(db, seedOptions.PersonId, seedOptions.RoleId, ct);
            await DatabaseSeeder.SeedOneLoginUserAsync(db, seedOptions.OneLoginSubject, seedOptions.OneLoginEmail,
                seedOptions.PersonId, ct);

            await db.SaveChangesAsync(ct);
        });
        return builder;
    }
}
