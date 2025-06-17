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
        };
    }
}
