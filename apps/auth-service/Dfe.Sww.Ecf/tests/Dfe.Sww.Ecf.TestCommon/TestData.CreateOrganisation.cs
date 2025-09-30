using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using Dfe.Sww.Ecf.TestCommon.Fakers;

namespace Dfe.Sww.Ecf.TestCommon;

public partial class TestData
{
    public async Task<Organisation> CreateOrganisation(
        string? organisationName = null)
    {
        var organisationFaker = new OrganisationFaker().WithCreatedOn(Clock.UtcNow).WithUpdatedOn(Clock.UtcNow);
        var organisation = organisationName is null
            ? organisationFaker.Generate()
            : organisationFaker.WithName(organisationName).Generate();

        organisation = await WithDbContext(async dbContext =>
        {
            dbContext.Organisations.Add(organisation);
            await dbContext.SaveChangesAsync();

            return organisation;
        });

        return organisation;
    }

    public async Task<List<Organisation>> CreateOrganisations(int count)
    {
        var results = new List<Organisation>();

        for (var i = 0; i < count; i++)
        {
            results.Add(await CreateOrganisation());
        }

        return results;
    }
}
