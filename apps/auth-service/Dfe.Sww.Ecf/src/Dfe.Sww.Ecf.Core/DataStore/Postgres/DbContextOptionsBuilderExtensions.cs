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

            var org = await db.Set<Organisation>()
                .FirstOrDefaultAsync(o => o.OrganisationId == orgId, ct);
            if (org == null)
            {
                org = new Organisation
                {
                    OrganisationId = orgId,
                    OrganisationName = "Test Organisation",
                    ExternalOrganisationId = 0
                };
                await db.AddAsync(org, ct);
            }

            var person = await db.Set<Person>().FirstOrDefaultAsync(p => p.PersonId == personId, ct);
            if (person == null)
            {
                person = new Person
                {
                    PersonId = personId,
                    FirstName = "Test",
                    LastName = "Coordinator",
                    EmailAddress = "test.coordinator@test-org.com",
                    CreatedOn = DateTime.UtcNow,
                    Status = PersonStatus.Active
                };
                await db.AddAsync(person, ct);
            }

            var personOrg = await db.Set<PersonOrganisation>()
                .FirstOrDefaultAsync(po => po.OrganisationId == orgId && po.PersonId == personId, ct);
            if (personOrg == null)
            {
                personOrg = new PersonOrganisation
                {
                    OrganisationId = orgId,
                    PersonId = personId
                };
                await db.AddAsync(personOrg, ct);
            }

            const int coordinatorRoleId = (int)RoleType.Coordinator;

            var personRole = await db.Set<PersonRole>()
                .FirstOrDefaultAsync(pr => pr.PersonId == personId && pr.RoleId == coordinatorRoleId, ct);
            if (personRole == null)
            {
                personRole = new PersonRole
                {
                    PersonId = personId,
                    RoleId = coordinatorRoleId
                };
                await db.AddAsync(personRole, ct);
            }

            await db.SaveChangesAsync(ct);
        });
        return builder;
    }
}
