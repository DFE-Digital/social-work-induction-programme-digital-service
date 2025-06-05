using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService.Models;
using Dfe.Sww.Ecf.Frontend.Models;

namespace Dfe.Sww.Ecf.Frontend.Mappers;

public class UserMapper : IModelMapper<Person, User>
{
    public User MapToBo(Person person)
    {
        return new User
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

    public Person MapFromBo(User user)
    {
        return new Person
        {
            PersonId = user.Id,
            FirstName = user.FirstName ?? string.Empty,
            LastName = user.LastName ?? string.Empty,
            SocialWorkEnglandNumber = user.SocialWorkEnglandNumber,
            EmailAddress = user.Email,
            Status = user.Status,
            Roles = user.Types ?? [],
            CreatedOn = user.CreatedAt
        };
    }
}
