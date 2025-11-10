using System.Collections.Immutable;
using Bogus;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.RegisterSocialWorker;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Builders;

public class AccountBuilder
{
    private readonly Faker<Account> _faker = new();

    public AccountBuilder()
    {
        _faker.RuleFor(a => a.Id, f => f.Random.Guid());
        _faker.RuleFor(a => a.CreatedAt, f => f.Date.Past());
        _faker.RuleFor(a => a.FirstName, f => f.Name.FirstName());
        _faker.RuleFor(a => a.MiddleNames, f => f.Name.FirstName());
        _faker.RuleFor(a => a.LastName, f => f.Name.LastName());
        _faker.RuleFor(a => a.Status, f => f.PickRandom<AccountStatus>());
        _faker.RuleFor(a => a.Email, f => f.Internet.Email());
        _faker.RuleFor(a => a.Types, f => [f.PickRandom<AccountType>()]);
        _faker.RuleFor(
            a => a.SocialWorkEnglandNumber,
            (f, current) =>
                current.Types?.Contains(AccountType.EarlyCareerSocialWorker) == true || current.Types?.Contains(AccountType.Assessor) == true
                    ? f.Random.Number(1, 1000).ToString()
                    : null
        );
        _faker.RuleFor(a => a.ProgrammeStartDate, f => DateOnly.FromDateTime(f.Date.Past()));
        _faker.RuleFor(a => a.ProgrammeEndDate, f => DateOnly.FromDateTime(f.Date.Future()));

        _faker.RuleFor(a => a.DateOfBirth, f => DateOnly.FromDateTime(f.Date.Past()));
        _faker.RuleFor(a => a.UserSex, f => f.PickRandom<UserSex>());
        _faker.RuleFor(a => a.GenderMatchesSexAtBirth, f => f.PickRandom<GenderMatchesSexAtBirth>());
        _faker.RuleFor(a => a.OtherGenderIdentity, f => f.Name.FirstName());
        _faker.RuleFor(a => a.EthnicGroup, f => f.PickRandom<EthnicGroup>());
        _faker.RuleFor(a => a.EthnicGroupWhite, f => f.PickRandom<EthnicGroupWhite>());
        _faker.RuleFor(a => a.OtherEthnicGroupWhite, f => f.Name.FirstName());
        _faker.RuleFor(a => a.EthnicGroupMixed, f => f.PickRandom<EthnicGroupMixed>());
        _faker.RuleFor(a => a.OtherEthnicGroupMixed, f => f.Name.FirstName());
        _faker.RuleFor(a => a.EthnicGroupAsian, f => f.PickRandom<EthnicGroupAsian>());
        _faker.RuleFor(a => a.OtherEthnicGroupAsian, f => f.Name.FirstName());
        _faker.RuleFor(a => a.EthnicGroupBlack, f => f.PickRandom<EthnicGroupBlack>());
        _faker.RuleFor(a => a.OtherEthnicGroupBlack, f => f.Name.FirstName());
        _faker.RuleFor(a => a.EthnicGroupOther, f => f.PickRandom<EthnicGroupOther>());
        _faker.RuleFor(a => a.OtherEthnicGroupOther, f => f.Name.FirstName());
        _faker.RuleFor(a => a.Disability, f => f.PickRandom<Disability>());
        _faker.RuleFor(a => a.SocialWorkEnglandRegistrationDate, f => DateOnly.FromDateTime(f.Date.Past()));
        _faker.RuleFor(a => a.HighestQualification, f => f.PickRandom<Qualification>());
        _faker.RuleFor(a => a.SocialWorkQualificationEndYear, f => f.Random.Number(1900, 2000));
        _faker.RuleFor(a => a.RouteIntoSocialWork, f => f.PickRandom<RouteIntoSocialWork>());
        _faker.RuleFor(a => a.OtherRouteIntoSocialWork, f => f.Name.FirstName());
        _faker.RuleFor(a => a.HasCompletedLoginAccountLinking, f => f.Random.Bool());
    }

    public AccountBuilder WithAddOrEditAccountDetailsData()
    {
        _faker.RuleFor(a => a.FirstName, f => f.Name.FirstName());
        _faker.RuleFor(a => a.MiddleNames, f => f.Name.FirstName());
        _faker.RuleFor(a => a.LastName, f => f.Name.LastName());
        _faker.RuleFor(a => a.Email, f => f.Internet.Email());
        _faker.RuleFor(a => a.ProgrammeStartDate, _ => null);
        _faker.RuleFor(a => a.ProgrammeEndDate, _ => null);

        _faker.RuleFor(a => a.DateOfBirth, _ => null);
        _faker.RuleFor(a => a.UserSex, _ => null);
        _faker.RuleFor(a => a.GenderMatchesSexAtBirth, _ => null);
        _faker.RuleFor(a => a.OtherGenderIdentity, _ => null);
        _faker.RuleFor(a => a.EthnicGroup, _ => null);
        _faker.RuleFor(a => a.EthnicGroupWhite, _ => null);
        _faker.RuleFor(a => a.OtherEthnicGroupWhite, _ => null);
        _faker.RuleFor(a => a.EthnicGroupMixed, _ => null);
        _faker.RuleFor(a => a.OtherEthnicGroupMixed, _ => null);
        _faker.RuleFor(a => a.EthnicGroupAsian, _ => null);
        _faker.RuleFor(a => a.OtherEthnicGroupAsian, _ => null);
        _faker.RuleFor(a => a.EthnicGroupBlack, _ => null);
        _faker.RuleFor(a => a.OtherEthnicGroupBlack, _ => null);
        _faker.RuleFor(a => a.EthnicGroupOther, _ => null);
        _faker.RuleFor(a => a.OtherEthnicGroupOther, _ => null);
        _faker.RuleFor(a => a.Disability, _ => null);
        _faker.RuleFor(a => a.SocialWorkEnglandRegistrationDate, _ => null);
        _faker.RuleFor(a => a.HighestQualification, _ => null);
        _faker.RuleFor(a => a.SocialWorkQualificationEndYear, _ => null);
        _faker.RuleFor(a => a.RouteIntoSocialWork, _ => null);
        _faker.RuleFor(a => a.OtherRouteIntoSocialWork, _ => null);
        _faker.RuleFor(a => a.Types, _ => null);
        return this;
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

    public AccountBuilder WithMiddleNames(string? middleNames)
    {
        _faker.RuleFor(x => x.MiddleNames, middleNames);
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
                current.Types?.Contains(AccountType.EarlyCareerSocialWorker) == true || current.Types?.Contains(AccountType.Assessor) == true
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

    public AccountBuilder WithIsFunded(bool isFunded)
    {
        _faker.RuleFor(a => a.IsFunded, _ => isFunded);

        return this;
    }

    public AccountBuilder WithIsStaff(bool isStaff)
    {
        _faker.RuleFor(a => a.IsStaff, _ => isStaff);

        return this;
    }

    public AccountBuilder WithStartDate(DateOnly startDate)
    {
        _faker.RuleFor(x => x.ProgrammeStartDate, startDate);
        return this;
    }

    public AccountBuilder WithEndDate(DateOnly endDate)
    {
        _faker.RuleFor(x => x.ProgrammeEndDate, endDate);
        return this;
    }

    public AccountBuilder WithDateOfBirth(DateOnly? dateOfBirth)
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

    public AccountBuilder WithEthnicGroup(EthnicGroup ethnicGroup)
    {
        _faker.RuleFor(a => a.EthnicGroup, _ => ethnicGroup);

        return this;
    }

    public AccountBuilder WithEthnicGroupWhite(EthnicGroupWhite ethnicGroupWhite)
    {
        _faker.RuleFor(a => a.EthnicGroupWhite, _ => ethnicGroupWhite);

        return this;
    }

    public AccountBuilder WithOtherEthnicGroupWhite(string otherEthnicGroupWhite)
    {
        _faker.RuleFor(a => a.OtherEthnicGroupWhite, _ => otherEthnicGroupWhite);

        return this;
    }

    public AccountBuilder WithEthnicGroupAsian(EthnicGroupAsian ethnicGroupAsian)
    {
        _faker.RuleFor(a => a.EthnicGroupAsian, _ => ethnicGroupAsian);

        return this;
    }

    public AccountBuilder WithOtherEthnicGroupAsian(string otherEthnicGroupAsian)
    {
        _faker.RuleFor(a => a.OtherEthnicGroupAsian, _ => otherEthnicGroupAsian);

        return this;
    }

    public AccountBuilder WithEthnicGroupBlack(EthnicGroupBlack ethnicGroupBlack)
    {
        _faker.RuleFor(a => a.EthnicGroupBlack, _ => ethnicGroupBlack);

        return this;
    }

    public AccountBuilder WithOtherEthnicGroupBlack(string otherEthnicGroupBlack)
    {
        _faker.RuleFor(a => a.OtherEthnicGroupBlack, _ => otherEthnicGroupBlack);

        return this;
    }

    public AccountBuilder WithEthnicGroupOther(EthnicGroupOther ethnicGroupOther)
    {
        _faker.RuleFor(a => a.EthnicGroupOther, _ => ethnicGroupOther);

        return this;
    }

    public AccountBuilder WithOtherEthnicGroupOther(string otherEthnicGroupOther)
    {
        _faker.RuleFor(a => a.OtherEthnicGroupOther, _ => otherEthnicGroupOther);

        return this;
    }

    public AccountBuilder WithIsDisabled(Disability isDisabled)
    {
        _faker.RuleFor(a => a.Disability, _ => isDisabled);

        return this;
    }

    public AccountBuilder WithSocialWorkerRegistrationDate(DateOnly socialWorkEnglandRegistrationDate)
    {
        _faker.RuleFor(a => a.SocialWorkEnglandRegistrationDate, _ => socialWorkEnglandRegistrationDate);

        return this;
    }

    public AccountBuilder WithHighestQualification(Qualification highestQualification)
    {
        _faker.RuleFor(a => a.HighestQualification, _ => highestQualification);

        return this;
    }

    public AccountBuilder WithSocialWorkQualificationEndYear(int socialWorkQualificationEndYear)
    {
        _faker.RuleFor(a => a.SocialWorkQualificationEndYear, _ => socialWorkQualificationEndYear);

        return this;
    }

    public AccountBuilder WithRouteIntoSocialWork(RouteIntoSocialWork routeIntoSocialWork)
    {
        _faker.RuleFor(a => a.RouteIntoSocialWork, _ => routeIntoSocialWork);

        return this;
    }

    public AccountBuilder WithOtherRouteIntoSocialWork(string otherRouteIntoSocialWork)
    {
        _faker.RuleFor(a => a.OtherRouteIntoSocialWork, _ => otherRouteIntoSocialWork);

        return this;
    }

    public AccountBuilder WithHasCompletedLoginAccountLinking(bool hasCompletedLoginAccountLinking)
    {
        _faker.RuleFor(a => a.HasCompletedLoginAccountLinking, _ => hasCompletedLoginAccountLinking);

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
