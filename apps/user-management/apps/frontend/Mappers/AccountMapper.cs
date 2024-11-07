using Dfe.Sww.Ecf.Frontend.HttpClients.AccountsService.Models;
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
            SocialWorkEnglandNumber = person.Trn,
            Email = person.EmailAddress,
            // Status = TODO
            // Types =  TODO
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
            Trn = account.SocialWorkEnglandNumber,
            EmailAddress = account.Email,
            // Status = TODO
            // Types =  TODO
            CreatedOn = account.CreatedAt
        };
    }
}
