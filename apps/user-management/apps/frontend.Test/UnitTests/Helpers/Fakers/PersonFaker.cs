using Bogus;
using Person = Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models.Person;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;

public sealed class PersonFaker : Faker<Person>
{
    public PersonFaker()
    {
        RuleFor(a => a.PersonId, f => f.Random.Guid());
        RuleFor(a => a.FirstName, f => f.Name.FirstName());
        RuleFor(a => a.LastName, f => f.Name.LastName());
        RuleFor(a => a.EmailAddress, f => f.Internet.Email());
        RuleFor(a => a.Trn, f => f.Random.Number().ToString());
        RuleFor(a => a.CreatedOn, f => f.Date.Past());
    }
}
