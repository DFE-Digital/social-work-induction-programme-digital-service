using System.Collections.Immutable;
using Bogus;
using Dfe.Sww.Ecf.Frontend.Models;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Builders;

public class UserBuilder
{
    private readonly Faker<User> _faker = new();

    public UserBuilder()
    {
        _faker.RuleFor(a => a.Id, f => f.Random.Guid());
        _faker.RuleFor(a => a.CreatedAt, f => f.Date.Past());
        _faker.RuleFor(a => a.FirstName, f => f.Name.FirstName());
        _faker.RuleFor(a => a.LastName, f => f.Name.LastName());
        _faker.RuleFor(a => a.Status, f => f.PickRandom<UserStatus>());
        _faker.RuleFor(a => a.Email, f => f.Internet.Email());
        _faker.RuleFor(a => a.Types, f => [f.PickRandom<UserType>()]);
        _faker.RuleFor(
            a => a.SocialWorkEnglandNumber,
            (f, current) =>
                current.Types?.Contains(UserType.EarlyCareerSocialWorker) == true
                    ? f.Random.Number(1, 1000).ToString()
                    : null
        );
    }

    public UserBuilder WithId(Guid id)
    {
        _faker.RuleFor(x => x.Id, id);
        return this;
    }

    public UserBuilder WithCreatedAt(DateTime createdAt)
    {
        _faker.RuleFor(x => x.CreatedAt, createdAt);
        return this;
    }

    public UserBuilder WithFirstName(string? firstName)
    {
        _faker.RuleFor(x => x.FirstName, firstName);
        return this;
    }

    public UserBuilder WithLastName(string? lastName)
    {
        _faker.RuleFor(x => x.LastName, lastName);
        return this;
    }

    public UserBuilder WithEmail(string? email)
    {
        _faker.RuleFor(x => x.Email, email);
        return this;
    }

    public UserBuilder WithStatus(UserStatus? status)
    {
        _faker.RuleFor(x => x.Status, status);
        return this;
    }

    public UserBuilder WithTypes(ImmutableList<UserType>? types)
    {
        _faker.RuleFor(x => x.Types, types);
        return this;
    }

    public UserBuilder WithSocialWorkEnglandNumber(string? socialWorkEnglandNumber)
    {
        _faker.RuleFor(
            x => x.SocialWorkEnglandNumber,
            (f, current) =>
                current.Types?.Contains(UserType.EarlyCareerSocialWorker) == true
                    ? socialWorkEnglandNumber
                    : null
        );
        return this;
    }

    public UserBuilder WithSocialWorkEnglandNumber()
    {
        _faker.RuleFor(a => a.SocialWorkEnglandNumber, f => f.Random.Number(1, 1000).ToString());

        return this;
    }

    public UserBuilder WithIsStaff(bool isStaff)
    {
        _faker.RuleFor(a => a.IsStaff, _ => isStaff);

        return this;
    }

    public User Build()
    {
        return _faker.Generate();
    }

    public List<User> BuildMany(int count)
    {
        return _faker.Generate(count);
    }
}
