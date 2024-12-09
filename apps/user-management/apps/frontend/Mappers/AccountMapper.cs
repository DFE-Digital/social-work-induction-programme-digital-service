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
            LastName = person.LastName,
            SocialWorkEnglandNumber = person.SocialWorkEnglandNumber,
            Email = person.EmailAddress,
            Status = person.Status,
            Types = person.Roles,
            CreatedAt = person.CreatedOn
        };
    }

    public Person MapFromBo(Account account)
    {
        return new Person
        {
            PersonId = account.Id,
            FirstName = account.FirstName ?? string.Empty,
            LastName = account.LastName ?? string.Empty,
            SocialWorkEnglandNumber = account.SocialWorkEnglandNumber,
            EmailAddress = account.Email,
            Status = account.Status,
            Roles = account.Types ?? [],
            CreatedOn = account.CreatedAt
        };
    }
}
