using System.Collections.Immutable;
using Bogus;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisation;
using Dfe.Sww.Ecf.Frontend.Models.RegisterSocialWorker;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Builders;

public class OrganisationBuilder
{
    private readonly Faker<Organisation> _faker = new();

    public OrganisationBuilder()
    {
        _faker.RuleFor(a => a.OrganisationId, f => f.Random.Guid());
        _faker.RuleFor(a => a.OrganisationName, f => f.Name.FirstName());
        _faker.RuleFor(a => a.ExternalOrganisationId, f => f.Random.Number(1, 1000));
        _faker.RuleFor(a => a.LocalAuthorityCode, f => f.Random.Number(1, 1000));
        _faker.RuleFor(a => a.Type, f => f.PickRandom<OrganisationType>());
    }

    public OrganisationBuilder WithRegion()
    {
        _faker.RuleFor(a => a.Region, f => f.Name.FirstName());
        return this;
    }

    public Organisation Build()
    {
        return _faker.Generate();
    }

    public List<Organisation> BuildMany(int count)
    {
        return _faker.Generate(count);
    }
}
