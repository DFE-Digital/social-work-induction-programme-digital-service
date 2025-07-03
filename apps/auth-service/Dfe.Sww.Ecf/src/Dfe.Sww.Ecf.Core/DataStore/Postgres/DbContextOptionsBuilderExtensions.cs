using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;

namespace Dfe.Sww.Ecf.Core.DataStore.Postgres;

public static class DbContextOptionsBuilderExtensions
{
    public static DbContextOptionsBuilder UseDatabaseSeeding(this DbContextOptionsBuilder builder)
    {
        builder.UseAsyncSeeding(async (db, _, ct) =>
        {
            var orgId = new Guid("00000000-0000-0000-0000-000000000001");
            var personId = new Guid("10000000-0000-0000-0000-000000000001");
            const int roleId = (int)RoleType.Coordinator;

            await DatabaseSeeder.SeedOrganisationAsync(db, orgId, ct);
            await DatabaseSeeder.SeedPersonAsync(db, personId, ct);
            await DatabaseSeeder.SeedPersonOrganisationAsync(db, orgId, personId, ct);
            await DatabaseSeeder.SeedPersonRoleAsync(db, personId, roleId, ct);

            await db.SaveChangesAsync(ct);
        });
        return builder;
    }
}
