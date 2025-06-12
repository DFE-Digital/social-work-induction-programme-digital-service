using System.Collections.Immutable;
using Bogus;
using Dfe.Sww.Ecf.Frontend.Models;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Builders;

public class AccountBuilder
{
    private readonly Faker<Account> _faker = new();

    public AccountBuilder()
    {
        _faker.RuleFor(a => a.Id, f => f.Random.Guid());
        _faker.RuleFor(a => a.CreatedAt, f => f.Date.Past());
        _faker.RuleFor(a => a.FirstName, f => f.Name.FirstName());
        _faker.RuleFor(a => a.MiddleNames, f => f.Name.LastName());
        _faker.RuleFor(a => a.LastName, f => f.Name.LastName());
        _faker.RuleFor(a => a.Status, f => f.PickRandom<AccountStatus>());
        _faker.RuleFor(a => a.Email, f => f.Internet.Email());
        _faker.RuleFor(a => a.Types, f => [f.PickRandom<AccountType>()]);
        _faker.RuleFor(
            a => a.SocialWorkEnglandNumber,
            (f, current) =>
                current.Types?.Contains(AccountType.EarlyCareerSocialWorker) == true
                    ? f.Random.Number(1, 1000).ToString()
                    : null
        );
        _faker.RuleFor(a => a.DateOfBirth, f => f.Date.Past());
        _faker.RuleFor(a => a.UserSex, f => f.PickRandom<UserSex>());
        _faker.RuleFor(a => a.GenderMatchesSexAtBirth, f => f.PickRandom<GenderMatchesSexAtBirth>());
    }

    public AccountBuilder WithId(Guid id)
    {
        _faker.RuleFor(x => x.Id, id);
        return this;
    }

    public AccountBuilder WithCreatedAt(DateTime createdAt)
    {
        _faker.RuleFor(x => x.CreatedAt, createdAt);
        return this;
    }

    public AccountBuilder WithFirstName(string? firstName)
    {
        _faker.RuleFor(x => x.FirstName, firstName);
        return this;
    }

    public AccountBuilder WithLastName(string? lastName)
    {
        _faker.RuleFor(x => x.LastName, lastName);
        return this;
    }

    public AccountBuilder WithEmail(string? email)
    {
        _faker.RuleFor(x => x.Email, email);
        return this;
    }

    public AccountBuilder WithStatus(AccountStatus? status)
    {
        _faker.RuleFor(x => x.Status, status);
        return this;
    }

    public AccountBuilder WithTypes(ImmutableList<AccountType>? types)
    {
        _faker.RuleFor(x => x.Types, types);
        return this;
    }

    public AccountBuilder WithSocialWorkEnglandNumber(string? socialWorkEnglandNumber)
    {
        _faker.RuleFor(
            x => x.SocialWorkEnglandNumber,
            (_, current) =>
                current.Types?.Contains(AccountType.EarlyCareerSocialWorker) == true
                    ? socialWorkEnglandNumber
                    : null
        );
        return this;
    }

    public AccountBuilder WithSocialWorkEnglandNumber()
    {
        _faker.RuleFor(a => a.SocialWorkEnglandNumber, f => f.Random.Number(1, 1000).ToString());

        return this;
    }

    public AccountBuilder WithIsStaff(bool isStaff)
    {
        _faker.RuleFor(a => a.IsStaff, _ => isStaff);

        return this;
    }

    public AccountBuilder WithDateOfBirth(DateTime? dateOfBirth)
    {
        _faker.RuleFor(a => a.DateOfBirth, _ => dateOfBirth);

        return this;
    }

    public AccountBuilder WithUserSex(UserSex userSex)
    {
        _faker.RuleFor(a => a.UserSex, _ => userSex);

        return this;
    }

    public AccountBuilder WithGenderMatchesSexAtBirth(GenderMatchesSexAtBirth? genderMatchesSexAtBirth)
    {
        _faker.RuleFor(a => a.GenderMatchesSexAtBirth, _ => genderMatchesSexAtBirth);

        return this;
    }

    public AccountBuilder WithNoRegistrationQuestions()
    {
        _faker.RuleFor(a => a.DateOfBirth, _ => null);

        return this;
    }

    public Account Build()
    {
        return _faker.Generate();
    }

    public List<Account> BuildMany(int count)
    {
        return _faker.Generate(count);
    }
}
