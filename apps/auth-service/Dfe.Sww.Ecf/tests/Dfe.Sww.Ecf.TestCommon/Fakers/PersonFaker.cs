using Bogus;
using Person = Dfe.Sww.Ecf.Core.DataStore.Postgres.Models.Person;

namespace Dfe.Sww.Ecf.TestCommon.Fakers;

public sealed class PersonFaker : Faker<Person>
{
    public PersonFaker()
    {
        RuleFor(a => a.PersonId, f => f.Random.Guid());
        RuleFor(a => a.FirstName, f => f.Person.FirstName);
        RuleFor(a => a.LastName, f => f.Person.LastName);
        RuleFor(a => a.DateOfBirth, f => DateOnly.FromDateTime(f.Person.DateOfBirth));
        RuleFor(a => a.EmailAddress, f => f.Internet.Email());
        RuleFor(a => a.Trn, f => f.Random.Number(1, 999999).ToString());
        RuleFor(a => a.CreatedOn, f => f.Date.Past().ToUniversalTime());
    }
}
