using Bogus;
using Dfe.Sww.Ecf.Frontend.Models;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Fakers;

public sealed class GetEscwRegisterChangeLinksFaker : Faker<EscwRegisterChangeLinks>
{
    public GetEscwRegisterChangeLinksFaker()
    {
        RuleFor(a => a.DateOfBirthChangeLink, f => f.Random.String());
        RuleFor(a => a.UserSexChangeLink, f => f.Name.FirstName());
        RuleFor(a => a.GenderIdentityChangeLink, f => f.Name.LastName());
        RuleFor(a => a.EthnicGroupChangeLink, f => f.Internet.Email());
        RuleFor(a => a.EthnicGroupingChangeLink, f => f.Random.String());
        RuleFor(a => a.DisabilityChangeLink, f => f.Random.String());
        RuleFor(a => a.SocialWorkEnglandRegistrationChangeLink, f => f.Random.String());
        RuleFor(a => a.HighestQualificationChangeLink, f =>f.Random.String());
        RuleFor(a => a.SocialWorkQualificationEndYearChangeLink, f => f.Random.String());
        RuleFor(a => a.RouteIntoSocialWorkChangeLink, f => f.Random.String());
    }
}
