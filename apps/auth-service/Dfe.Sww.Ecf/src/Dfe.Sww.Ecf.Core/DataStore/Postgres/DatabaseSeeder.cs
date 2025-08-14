using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;

namespace Dfe.Sww.Ecf.Core.DataStore.Postgres;

public static class DatabaseSeeder
{
    public static async Task SeedOrganisationAsync(DbContext db, Guid orgId, string? orgName, CancellationToken ct)
    {
        await db.Set<Organisation>().AddIfNotExistsAsync(
            o => o.OrganisationId == orgId,
            () => new Organisation
            {
                OrganisationId = orgId,
                OrganisationName = orgName ?? "Test Organisation",
                ExternalOrganisationId = 0
            },
            ct
        );
    }

    public static async Task SeedPersonAsync(DbContext db, Guid personId, string email, CancellationToken ct)
    {
        await db.Set<Person>().AddIfNotExistsAsync(
            p => p.PersonId == personId,
            () => new Person
            {
                PersonId = personId,
                FirstName = "Test",
                LastName = "User",
                EmailAddress = email,
                CreatedOn = DateTime.UtcNow,
                Status = PersonStatus.Active
            },
            ct
        );
    }

    public static async Task SeedPersonOrganisationAsync(DbContext db, Guid orgId, Guid personId, CancellationToken ct)
    {
        await db.Set<PersonOrganisation>().AddIfNotExistsAsync(
            po => po.OrganisationId == orgId && po.PersonId == personId,
            () => new PersonOrganisation
            {
                OrganisationId = orgId,
                PersonId = personId
            },
            ct
        );
    }

    public static async Task SeedPersonRoleAsync(DbContext db, Guid personId, int roleId, CancellationToken ct)
    {
        await db.Set<PersonRole>().AddIfNotExistsAsync(
            pr => pr.PersonId == personId && pr.RoleId == roleId,
            () => new PersonRole
            {
                PersonId = personId,
                RoleId = roleId
            },
            ct
        );
    }
}
