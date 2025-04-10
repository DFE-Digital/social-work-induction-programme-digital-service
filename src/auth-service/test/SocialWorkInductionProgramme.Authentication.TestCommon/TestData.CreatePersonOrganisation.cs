using SocialWorkInductionProgramme.Authentication.Core.DataStore.Postgres.Models;

namespace SocialWorkInductionProgramme.Authentication.TestCommon;

public partial class TestData
{
    public async Task<PersonOrganisation> CreatePersonOrganisation(
        CreatePersonResult person,
        Organisation organisation,
        DateOnly startDate,
        DateOnly? endDate = null)
    {

        var personEmployment = await WithDbContext(async dbContext =>
        {
            var personOrganisation = new PersonOrganisation
            {
                PersonOrganisationId = Guid.NewGuid(),
                PersonId = person.PersonId,
                OrganisationId = organisation.OrganisationId,
                StartDate = startDate,
                EndDate = endDate,
                CreatedOn = Clock.UtcNow,
                UpdatedOn = Clock.UtcNow,

            };

            dbContext.PersonOrganisations.Add(personOrganisation);
            await dbContext.SaveChangesAsync();

            return personOrganisation;
        });

        return personEmployment;
    }
}
