using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;

namespace Dfe.Sww.Ecf.TestCommon;

public partial class TestData
{
    private static int _lastExternalOrganisationId = 100000;

    public int GenerateExternalOrganisationId() => Interlocked.Increment(ref _lastExternalOrganisationId);

    public async Task<Organisation> CreateOrganisation(
        string organisationName)
    {
        var externalOrganisationId = GenerateExternalOrganisationId();

        var organisation = await WithDbContext(async dbContext =>
        {
            var organisation = new Organisation
            {
                OrganisationId = Guid.NewGuid(),
                OrganisationName = organisationName,
                ExternalOrganisationId = externalOrganisationId,
                CreatedOn = Clock.UtcNow,
                UpdatedOn = Clock.UtcNow
            };

            dbContext.Organisations.Add(organisation);
            await dbContext.SaveChangesAsync();

            return organisation;
        });

        return organisation;
    }
}
