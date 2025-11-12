using Bogus;
using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models.Organisations;

namespace Dfe.Sww.Ecf.TestCommon.Fakers;

public sealed class OrganisationFaker : Faker<Organisation>
{
    private static int _lastExternalOrganisationId = 100000;

    public OrganisationFaker()
    {
        var primaryCoordinator = new PersonFaker().Generate();
        RuleFor(a => a.OrganisationId, f => f.Random.Guid());
        RuleFor(a => a.OrganisationName, f => f.Company.CompanyName());
        RuleFor(a => a.PhoneNumber, f => f.Phone.PhoneNumber("+44##########"));
        RuleFor(a => a.Region, f => f.Address.County());
        RuleFor(a => a.LocalAuthorityCode, f => f.Random.Number(100, 999));
        RuleFor(
            a => a.ExternalOrganisationId,
            _ => Interlocked.Increment(ref _lastExternalOrganisationId)
        );
        RuleFor(a => a.CreatedOn, f => f.Date.Past().ToUniversalTime());
        RuleFor(a => a.UpdatedOn, f => f.Date.Recent().ToUniversalTime());
        RuleFor(a => a.Type, f => f.PickRandom<OrganisationType>());
        RuleFor(a => a.PrimaryCoordinator, _ => primaryCoordinator);
        RuleFor(a => a.PrimaryCoordinatorId, _ => primaryCoordinator.PersonId);
    }

    public OrganisationFaker WithName(string name)
    {
        RuleFor(a => a.OrganisationName, _ => name);
        return this;
    }

    public OrganisationFaker WithCreatedOn(DateTime createdOn)
    {
        RuleFor(a => a.CreatedOn, _ => createdOn);
        return this;
    }

    public OrganisationFaker WithUpdatedOn(DateTime updatedOn)
    {
        RuleFor(a => a.UpdatedOn, _ => updatedOn);
        return this;
    }
}
