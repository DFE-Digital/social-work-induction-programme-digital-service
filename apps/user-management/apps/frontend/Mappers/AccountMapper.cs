using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Dfe.Sww.Ecf.Frontend.Models;

namespace Dfe.Sww.Ecf.Frontend.Mappers;

public class AccountMapper : IModelMapper<Person, Account>
{
    public Account MapToBo(Person person)
    {
        return new Account
        {
            Id = person.PersonId,
            FirstName = person.FirstName,
            MiddleNames = person.MiddleName,
            LastName = person.LastName,
            SocialWorkEnglandNumber = person.SocialWorkEnglandNumber,
            Email = person.EmailAddress,
            Status = person.Status,
            Types = person.Roles,
            CreatedAt = person.CreatedOn,
            ProgrammeStartDate = person.ProgrammeStartDate,
            ProgrammeEndDate = person.ProgrammeEndDate,
            DateOfBirth = person.DateOfBirth,
            UserSex = person.UserSex,
            GenderMatchesSexAtBirth = person.GenderMatchesSexAtBirth,
            OtherGenderIdentity = person.OtherGenderIdentity,
            EthnicGroup = person.EthnicGroup,
            EthnicGroupWhite = person.EthnicGroupWhite,
            OtherEthnicGroupWhite = person.OtherEthnicGroupWhite,
            EthnicGroupMixed = person.EthnicGroupMixed,
            OtherEthnicGroupMixed = person.OtherEthnicGroupMixed,
            EthnicGroupAsian = person.EthnicGroupAsian,
            OtherEthnicGroupAsian = person.OtherEthnicGroupAsian,
            EthnicGroupBlack = person.EthnicGroupBlack,
            OtherEthnicGroupBlack = person.OtherEthnicGroupBlack,
            EthnicGroupOther = person.EthnicGroupOther,
            OtherEthnicGroupOther = person.OtherEthnicGroupOther,
            Disability = person.Disability,
            SocialWorkEnglandRegistrationDate = person.SocialWorkEnglandRegistrationDate,
            HighestQualification = person.HighestQualification,
            SocialWorkQualificationEndYear = person.SocialWorkQualificationEndYear,
            RouteIntoSocialWork = person.RouteIntoSocialWork,
            OtherRouteIntoSocialWork = person.OtherRouteIntoSocialWork
        };
    }

    public Person MapFromBo(Account account)
    {
        return new Person
        {
            PersonId = account.Id,
            FirstName = account.FirstName ?? string.Empty,
            MiddleName = account.MiddleNames,
            LastName = account.LastName ?? string.Empty,
            SocialWorkEnglandNumber = account.SocialWorkEnglandNumber,
            EmailAddress = account.Email,
            Status = account.Status,
            Roles = account.Types ?? [],
            CreatedOn = account.CreatedAt,
            ProgrammeStartDate = account.ProgrammeStartDate,
            ProgrammeEndDate = account.ProgrammeEndDate,
            DateOfBirth = account.DateOfBirth,
            UserSex = account.UserSex,
            GenderMatchesSexAtBirth = account.GenderMatchesSexAtBirth,
            OtherGenderIdentity = account.OtherGenderIdentity,
            EthnicGroup = account.EthnicGroup,
            EthnicGroupWhite = account.EthnicGroupWhite,
            OtherEthnicGroupWhite = account.OtherEthnicGroupWhite,
            EthnicGroupMixed = account.EthnicGroupMixed,
            OtherEthnicGroupMixed = account.OtherEthnicGroupMixed,
            EthnicGroupAsian = account.EthnicGroupAsian,
            OtherEthnicGroupAsian = account.OtherEthnicGroupAsian,
            EthnicGroupBlack = account.EthnicGroupBlack,
            OtherEthnicGroupBlack = account.OtherEthnicGroupBlack,
            EthnicGroupOther = account.EthnicGroupOther,
            OtherEthnicGroupOther = account.OtherEthnicGroupOther,
            Disability = account.Disability,
            SocialWorkEnglandRegistrationDate = account.SocialWorkEnglandRegistrationDate,
            HighestQualification = account.HighestQualification,
            SocialWorkQualificationEndYear = account.SocialWorkQualificationEndYear,
            RouteIntoSocialWork = account.RouteIntoSocialWork,
            OtherRouteIntoSocialWork = account.OtherRouteIntoSocialWork
        };
    }
}
